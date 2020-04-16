namespace Actor.Server
{
    public class WorkerStatus
    {
        public WorkerReadyState State { get; set; }
        public int TimeToLive { get; set; }
    }
}
