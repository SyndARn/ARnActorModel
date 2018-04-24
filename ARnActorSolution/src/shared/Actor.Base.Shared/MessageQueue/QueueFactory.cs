using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.Base
{
    public enum QueueStyle { None, LockFree, Locking, Ring }

    public class QueueFactory<T>
    {

        public static QueueFactory<T> Current { get; } = new Lazy<QueueFactory<T>>(true).Value;
        public QueueStyle Style { get; set; }

        public IMessageQueue<T> GetQueue()
        {
            switch (Style)
            {
                case QueueStyle.LockFree: return new LockFreeQueue<T>();
                case QueueStyle.Locking: return new LockQueue<T>();
                case QueueStyle.Ring: return new RingQueue<T>();
                default: return new LockFreeQueue<T>();
            }
        }
    }
}
