using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{
    public class DictionaryBehavior<TKey,TValue> : Behaviors, IDictionaryActor<TKey,TValue>
    {
        private readonly Dictionary<TKey, TValue> fDico = new Dictionary<TKey, TValue>();

        public DictionaryBehavior()
        {
            var bhv1 = new Behavior<TKey, TValue>( (k, v) => fDico[k] = v);
            var bhv2 = new Behavior<IActor, TKey>((a, k) =>
                {
                    bool result = fDico.TryGetValue(k, out TValue v);
                    a.SendMessage(result, k, v);
                });
            var bhv3 = new Behavior<TKey>(k => fDico.Remove(k));
            AddBehavior(bhv1);
            AddBehavior(bhv2);
            AddBehavior(bhv3);
        }

        public void AddKeyValue(TKey key, TValue value)
        {
            LinkedActor.SendMessage(key, value);
        }

        public IFuture<bool, TKey, TValue> GetKeyValue(TKey key)
        {
            var future = new Future<bool, TKey, TValue>();
            LinkedActor.SendMessage((IActor)future, key);
            return future;
        }

        public void RemoveKey(TKey key)
        {
            LinkedActor.SendMessage(key);
        }
    }

    public class DictionaryActor<TKey,TValue> : BaseActor, IDictionaryActor<TKey, TValue>
    {
        private readonly IDictionaryActor<TKey, TValue> fServiceDictionary;

        public DictionaryActor() : base()
        {
            DictionaryBehavior<TKey, TValue> lServiceDictionary = new DictionaryBehavior<TKey, TValue>();
            fServiceDictionary = lServiceDictionary;
            Become(lServiceDictionary);
        }

        public void AddKeyValue(TKey key, TValue value)
        {
            fServiceDictionary.AddKeyValue(key, value);
        }

        public IFuture<bool, TKey, TValue> GetKeyValue(TKey key)
        {
            return fServiceDictionary.GetKeyValue(key);
        }

        public void RemoveKey(TKey key)
        {
            fServiceDictionary.RemoveKey(key);
        }
    }
}
