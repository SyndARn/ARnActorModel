using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Actor.Base
{

    /// <summary>
    /// cCompositeQueue
    ///     ConcurrentQueue for the sync part
    ///     List for the non sync part
    ///     Allow search into the list part
    ///     when message issued from the concurrent queue
    ///     was rejected from a previous filter
    /// </summary>
    class CompositeQueue<T>
    {
        //private SpinLock 
        private List<T> fMTHQueue;
        // private ConcurrentQueue<T> fQueue;
        private Queue<T> fLocalQueue;
        private SpinLock fLock = new SpinLock(true);
        public CompositeQueue()
        {
            // fQueue = new ConcurrentQueue<T>();
            fMTHQueue = new List<T>();           
            fLocalQueue = new Queue<T>();
        }
        public void Enqueue(T msg)
        {
            bool fLockTaken = false;
            do
            {
                fLock.Enter(ref fLockTaken);
            } while (!fLockTaken);
            fMTHQueue.Add(msg);
            fLock.Exit();
            // fQueue.Enqueue(msg);
            // fQueue.Add(msg);
        }
        public int PostPoneCount
        {
            get { return fLocalQueue.Count; }
        }
        public bool TryDequeue(out T msg)
        {
            if (fLocalQueue.Count > 0)
            {
                msg = fLocalQueue.Dequeue();
                return true;
            }
            else
            {
                bool fLockTaken = false;
                do
                {
                    fLock.Enter(ref fLockTaken);
                } while (!fLockTaken);
                if (fMTHQueue.Count > 0)
                    {
                        foreach (T t in fMTHQueue)
                        {
                            fLocalQueue.Enqueue(t);
                        }
                        fMTHQueue.Clear();
                    }
                fLock.Exit();
                if (fLocalQueue.Count > 0)
                {
                    msg = fLocalQueue.Dequeue();
                    return true;
                }
                msg = default(T);
                return false;
                // return fQueue.TryTake(out msg) ; // T ryPop(out msg);// 
                // return fQueue.TryDequeue(out msg);
            }
        }

        public void PostPone(IEnumerable<T> l)
        {
            foreach (T t in l)
                fLocalQueue.Enqueue(t);
        }

        public void PostPone(T msg)
        {
            fLocalQueue.Enqueue(msg);
        }
    }
}
