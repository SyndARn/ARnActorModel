using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.Base
{
    public enum QueueStyle { None, LockFree, Locking }

    public static class QueueFactory<T>
    {
        public static QueueStyle Style { get; set; }
        public static IMessageQueue<T> Cast()
        {
            switch (Style)
            {
                case QueueStyle.LockFree: return new LockFreeQueue<T>();
                case QueueStyle.Locking: return new LockQueue<T>();
                default: return new LockFreeQueue<T>();
            }
        }
    }
}
