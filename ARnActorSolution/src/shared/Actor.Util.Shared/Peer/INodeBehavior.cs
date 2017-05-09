using Actor.Base;

namespace Actor.Util
{
    public interface INodeBehavior<TKey, TValue>
    {
        void StoreNode(TKey key, TValue value);
        IFuture<TValue> GetNode(TKey key);
        void DeleteNode(TKey key);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IFuture<HashKey> GetHashKey();
    }

}
