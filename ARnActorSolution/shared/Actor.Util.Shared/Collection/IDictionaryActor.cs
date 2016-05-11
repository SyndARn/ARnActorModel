using System;
using Actor.Base;

namespace Actor.Util
{
    public interface IDictionaryActor<TKey, TValue>
    {
        void AddKeyValue(TKey key, TValue value);
        Future<Tuple<bool, TKey, TValue>> GetKeyValue(TKey key);
    }
}