using Actor.Base;

namespace Actor.Server
{
    public class RequestStatus<T>
    {
        public ActorTag Tag { get; set; }
        public RequestState State { get; set; }
        public T Data { get; set; }
    }
}
