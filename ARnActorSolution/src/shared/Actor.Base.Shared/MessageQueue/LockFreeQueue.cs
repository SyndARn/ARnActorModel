using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Actor.Base
{
    public class LockFreeQueue<T> : IMessageQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        public void Add(T item)
        {
            _queue.Enqueue(item);
        }

        public int Count()
        {
            return _queue.Count;
        }

        public bool TryTake(out T item)
        {
            return _queue.TryDequeue(out item);
        }
    }
}
