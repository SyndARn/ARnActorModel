using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Actor.Base
{

    class ActorMailBox : ActorMailBox<Object>
    {
    }

    /// <summary>
    /// ActorMailBox
    ///   This mailbox works with the following guiding principles :
    ///     ConcurrentQueue are threadsafe, but somehow slow, 
    ///     Local queue are fast but unsafe.
    ///     When we need a message (GetMessage ...)
    ///     we first look on local queue (postpone)
    ///     if nothing available, we fill out the concurrent queue into the postpone queue
    ///     this way, access to ConcurrentQueue by this actor is reduced to only when needed
    /// </summary>
    class ActorMailBox<T> // : IDisposable
    {
        private ConcurrentQueue<T> fQueue = new ConcurrentQueue<T>(); // all actors may push here, only this one may dequeue
        private Queue<T> fPostpone = new Queue<T>(); // only this one use it, buffer from other queues.
        private Queue<T> fRunMissed = new Queue<T>(); // only this one use it in run mode
        private Queue<T> fReceiveMissed = new Queue<T>(); // only this one use it, in receivemode

        public ActorMailBox()
        {
        }

        public void AddReceiveMiss(T aMessage)
        {
            fReceiveMissed.Enqueue(aMessage);
        }

        public void AddRunMiss(T aMessage)
        {
            fRunMissed.Enqueue(aMessage);
        }

        public int RefreshFromRunMissed()
        {
            int i = 0;
            while (fRunMissed.Count >0)
            {
                fPostpone.Enqueue(fRunMissed.Dequeue()) ;
                i++ ;
            }
            return i;
        }

        public int RefreshFromReceiveMissed()
        {
            int i = 0;
            while (fReceiveMissed.Count >0)
            {
                fPostpone.Enqueue(fReceiveMissed.Dequeue());
                i++;
            }
            return i;
        }

        public void AddMessage(T aMessage)
        {
            fQueue.Enqueue(aMessage);
        }

        public T GetMessage()
        {
            if (fPostpone.Count>0)
            {
                return fPostpone.Dequeue();
            }
            else
            {
                T val = default(T);
                while (fQueue.TryDequeue(out val))
                    fPostpone.Enqueue(val);

                if (fPostpone.Count>0)
                {
                    return fPostpone.Dequeue();
                }
            }
            return default(T);
        }

    }

}
