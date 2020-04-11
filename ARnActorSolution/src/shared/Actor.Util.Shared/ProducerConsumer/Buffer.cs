using System.Collections.Generic;
using Actor.Base;

namespace Actor.Util
{
    public class Buffer<T> : FsmActor<string, Work<T>>
    {
        private readonly Queue<Consumer<T>> _consList = new Queue<Consumer<T>>();
        private readonly Queue<Work<T>> _workList = new Queue<Work<T>>();

        public Buffer(IEnumerable<Consumer<T>> someConsumers) : base()
        {
            CheckArg.IEnumerable(someConsumers);
            foreach (var item in someConsumers)
            {
                _consList.Enqueue(item);
                item.Buffer = this;
            }

            var bhv = new FsmBehaviors<string, Work<T>>();

            bhv
                .AddRule("BufferEmpty", null,
                t =>
                {
                    if (_consList.Count != 0)
                    {
                        var cons = _consList.Dequeue();
                        cons.SendMessage(t);
                    }
                    else
                    {
                        _workList.Enqueue(t);
                    }
                }, "BufferNotEmpty")
                .AddRule("BufferNotEmpty", t => _workList.Count != 0,
                t =>
                {
                    if (_consList.Count != 0)
                    {
                        var cons = _consList.Dequeue();
                        cons.SendMessage(t);
                    }
                    else
                    {
                        _workList.Enqueue(t);
                    }
                },
                "BufferNotEmpty");
        }
    }
}
