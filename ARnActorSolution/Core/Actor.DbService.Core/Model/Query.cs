using System.Collections.Generic;
using Actor.Base;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Actor.DbService.Core.Model
{
    public abstract class Query : BaseActor, IQuery
    {
        protected string QueryName { get; private set; }
        public string Uuid { get; } = Guid.NewGuid().ToString();

        protected int TotalMsg { get; set; }
        protected int ReceivedMsg { get; set; }

        private int streamStart = 0;
        private int streamEnd = 0;

        public Query()
        {
            QueryName = ToString();
        }

        public void StopQuery()
        {
            Interlocked.Increment(ref streamStart);
            Interlocked.Increment(ref streamEnd);
        }

        public void Launch(IActor asker, IndexRouter router)
        {
            CheckArg.Actor(router);
            var response = new Response(router, asker, this);
            Become(new Behavior<Response>(r => DoProcessQuery(r)));
            SendMessage(response);
        }

        public IEnumerable<Field> Launch(IndexRouter router)
        {
            CheckArg.Actor(router);
            ConcurrentQueue<Field> bc = new ConcurrentQueue<Field>();
            var spin = new SpinWait();
            var asker = new BaseActor(new Behavior<string, IEnumerable<Field>>
                (
                (s, fs) =>
                {
                    foreach (var item in fs)
                    {
                        bc.Enqueue(item);
                    }
                    ReceivedMsg++;
                    if (ReceivedMsg == 1)
                    {
                        Interlocked.Increment(ref streamStart);
                    }
                    if (ReceivedMsg >= TotalMsg)
                    {
                        Interlocked.Increment(ref streamEnd);
                    }
                }
                ));
            var response = new Response(router, asker, this);
            Become(new Behavior<Response>(r => DoProcessQuery(r)));
            SendMessage(response);
            while (Interlocked.CompareExchange(ref streamStart, streamStart, 0) == 0)
            {
                spin.SpinOnce();
            }

            do
            {
                while (bc.TryDequeue(out Field field))
                {
                    yield return field;
                };
                spin.SpinOnce();
            }
            while (Interlocked.CompareExchange(ref streamEnd, streamEnd, 0) == 0);
        }

        protected abstract void DoProcessQuery(Response response);
    }
}
