using Actor.Base;

namespace Actor.Util
{
    public interface IActorProxy
    {
        void Store(string aData);
        IFuture<string> Retrieve();
    }
}
