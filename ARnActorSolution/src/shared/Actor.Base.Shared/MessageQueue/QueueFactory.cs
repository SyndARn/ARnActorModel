using System;

namespace Actor.Base
{
    public enum QueueStyle { None, LockFree, Locking, Ring, Buffer }

    public class QueueFactory<T>
    {
        public QueueStyle Style { get; set; } = QueueStyle.LockFree;

        public IMessageQueue<T> GetQueue()
        {
            switch (Style)
            {
                case QueueStyle.LockFree: return new LockFreeQueue<T>();
                case QueueStyle.Locking: return new LockQueue<T>();
                case QueueStyle.Ring: return new RingQueue<T>();
                case QueueStyle.Buffer: return new BufferQueue<T>();
                default: return new LockFreeQueue<T>();
            }
        }
    }
}
