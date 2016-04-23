using System;
using Actor.Base;

namespace Actor.Util
{
    public interface IDictionaryActor<K, V>
    {
        void AddKV(K K, V V);
        Future<Tuple<bool, K, V>> GetKV(K k);
    }
}