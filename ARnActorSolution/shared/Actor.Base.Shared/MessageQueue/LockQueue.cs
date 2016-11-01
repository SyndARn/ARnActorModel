using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.Base
{
    public class LockQueue<T> : IMessageQueue<T>
    {
        private Queue<T> fQueue = new Queue<T>();
        private object fLock = new object();
        public LockQueue()
        {

        }

        public void Add(T item)
        {
            lock (fLock)
            {
                fQueue.Enqueue(item);
            }
        }

        public bool TryTake(out T item)
        {
            lock (fLock)
            {
                if (fQueue.Count == 0)
                {
                    item = default(T);
                    return false;
                }
                item = fQueue.Dequeue();
                return true;
            }
        }

        public int Count()
        {
            lock (fLock)
            {
                return fQueue.Count;
            }
        }
    }
}
