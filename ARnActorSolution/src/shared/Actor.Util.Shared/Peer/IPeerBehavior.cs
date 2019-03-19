using Actor.Base;

namespace Actor.Util
{
    public interface IPeerBehavior<TKey,TValue>
    {
        void FindPeer(TKey key, IFuture<IPeerActor<TKey,TValue>> actor);
        void NewPeer(IPeerActor<TKey,TValue> actor, HashKey hash);
    }
}
