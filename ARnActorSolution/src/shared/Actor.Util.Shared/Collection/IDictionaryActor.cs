using System;
using System.Collections.Generic;
using Actor.Base;

namespace Actor.Util
{
    public interface IDictionaryActor<TKey, TValue>
    {
        void AddKeyValue(TKey key, TValue value);
        IFuture<bool, TKey, TValue> GetKeyValue(TKey key);
        void RemoveKey(TKey key);
        IFuture<IEnumerable<KeyValuePair<TKey, TValue>>> AsEnumerable();
    }
}