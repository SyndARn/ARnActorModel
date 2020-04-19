namespace Actor.Base
{
    public class QueueFactory<T>
    {
        public enum QueueStyle
        {
            None,
            LockFree,
            Locking,
            Ring,
            Buffer
        }

        public QueueStyle Style { get; set; } = QueueStyle.LockFree;
        public static QueueFactory<T> Current = new QueueFactory<T>();

        public IMessageQueue<T> GetQueue()
        {
            switch (Style)
            {
                case QueueStyle.LockFree:
                    return new LockFreeQueue<T>();
                case QueueStyle.Locking:
                    return new LockQueue<T>();
                case QueueStyle.Ring:
                    return new RingQueue<T>();
                case QueueStyle.Buffer:
                    return new BufferQueue<T>();
                default:
                    return new LockFreeQueue<T>();
            }
        }
    }
}
