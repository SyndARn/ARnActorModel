using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading ;
using System.Collections.Concurrent;

namespace Actor.Base
{
    class ManyWriterOneReaderQueue<T>
    {
        Queue<T> fPostPoneList = new Queue<T>();
        Queue<T> fConcurrentQueue = new Queue<T>();
        private int fInQueue = 0;

        public ManyWriterOneReaderQueue()
        {
        }

        public void Enqueue(T aT)
        {
            SpinWait sw = new SpinWait();
            while (Interlocked.CompareExchange(ref fInQueue, 1, 0) != 0)
            {
                sw.SpinOnce();
            }
            fConcurrentQueue.Enqueue(aT);
            Interlocked.Exchange(ref fInQueue, 0);
        }



        public bool TryDequeue(out T msg)
        {
            // local
                if (fPostPoneList.Count > 0)
                {
                    msg = fPostPoneList.Dequeue();
                    return true;
                }
            
            // Threading
                SpinWait sw = new SpinWait();
                while (Interlocked.CompareExchange(ref fInQueue, 1, 0) != 0)
                {
                    sw.SpinOnce();
                }
                while (fConcurrentQueue.Count > 0)
                {
                    fPostPoneList.Enqueue(fConcurrentQueue.Dequeue());
                }
                Interlocked.Exchange(ref fInQueue, 0);
                    // nothing
            if (fPostPoneList.Count > 0)
            {
                msg = fPostPoneList.Dequeue();
                return true;
            }
            msg = default(T) ;
            return false ;
        }

        public void Postpone(List<T> aList)
        {
            foreach (T aT in aList)
                fPostPoneList.Enqueue(aT);
        }

        public int PostponeCount
        {
            get
            {
                return fPostPoneList.Count;
            }
        }

    }
}
