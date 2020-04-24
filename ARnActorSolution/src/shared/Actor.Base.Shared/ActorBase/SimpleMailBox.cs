using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Actor.Base
{
    public class SimpleMailBox<T> : IActorMailBox<T>
    {
        private readonly IMessageQueue<T> _queue; // all actors may push here, only this one may dequeue
        private readonly Queue<T> _missed; // only this one use it in run mode

        private static readonly QueueFactory<T> _factory = new QueueFactory<T>();

        public SimpleMailBox()
        {
            _queue = _factory.GetQueue();
            _missed = new Queue<T>();
        }

        public bool IsEmpty => _queue.Count() == 0;

        public void AddMiss(T aMessage) => _missed.Enqueue(aMessage);

        public int RefreshFromMissed()
        {
            int i = 0;
            while (_missed.Count > 0) 
            {
                _queue.Add(_missed.Dequeue());
                i++;
            }
            return i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddMessage(T aMessage) => _queue.Add(aMessage);

        public T GetMessage()
        {
            _queue.TryTake(out T val);
            return val;
        }
    }
}
