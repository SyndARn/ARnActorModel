using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.Base
{
    public class RingQueue<T> : IMessageQueue<T>
    {
        private RingBuffer<T> fRingBuffer = new RingBuffer<T>();
        private Object fLock = new Object();
        public void Add(T item)
        {
            lock (fLock)
            {
                fRingBuffer.Add(item);
            }
        }

        public int Count()
        {
            lock (fLock)
            {
                return fRingBuffer.Count();
            }
        }

        public bool TryTake(out T item)
        {
            lock (fLock)
            {
                return fRingBuffer.TryTake(out item);
            }
        }
    }

    public class RingBuffer<T>
    {
        private List<T> fList = new List<T>(128);
        private int fHead;
        private int fTail;
        public int Count()
        {
            return fHead - fTail;
        }
        public void Add(T aT)
        {
            if (fHead == fList.Count)
            {
                fList.Add(aT);
                fHead++;
            }
            else
            {
                fList[fHead] = aT;
                fHead++;
            }
        }
        public bool TryTake(out T item)
        {
            if (fHead > fTail)
            {
                item = fList[fTail];
                fList[fTail] = default(T);
                fTail++;
                if (fTail == fHead)
                {
                    fTail = 0;
                    fHead = 0;
                }
                return true;
            }
            item = default(T);
            return false;
        }
    }

}
