using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Actor.Base
{
    public class LockFreeQueue<T> : IMessageQueue<T>
    {
        private ConcurrentQueue<T> fQueue = new ConcurrentQueue<T>();

        public void Add(T item)
        {
            fQueue.Enqueue(item);
        }

        public int Count()
        {
            return fQueue.Count;
        }

        public bool TryTake(out T item)
        {
            item = default(T);
            return fQueue.TryDequeue(out item);
        }
    }
}
