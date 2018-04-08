using System.Collections.Generic;
using Actor.Base;

namespace Actor.Util
{
    public class Buffer<T> : FsmActor<string, Work<T>>
    {
        private readonly Queue<Consumer<T>> ConsList = new Queue<Consumer<T>>();
        private readonly Queue<Work<T>> WorkList = new Queue<Work<T>>();

        public Buffer(IEnumerable<Consumer<T>> someConsumers) : base()
        {
            CheckArg.IEnumerable(someConsumers);
            foreach (var item in someConsumers)
            {
                ConsList.Enqueue(item);
                item.Buffer = this;
            }

            var bhv = new FsmBehaviors<string, Work<T>>();

            bhv
                .AddRule("BufferEmpty", null,
                t =>
                {
                    if (ConsList.Count != 0)
                    {
                        var cons = ConsList.Dequeue();
                        cons.SendMessage(t);
                    }
                    else
                    {
                        WorkList.Enqueue(t);
                    }
                }, "BufferNotEmpty")
                .AddRule("BufferNotEmpty", t => WorkList.Count != 0,
                t =>
                {
                    if (ConsList.Count != 0)
                    {
                        var cons = ConsList.Dequeue();
                        cons.SendMessage(t);
                    }
                    else
                    {
                        WorkList.Enqueue(t);
                    }
                },
                "BufferNotEmpty");
        }
    }
}
