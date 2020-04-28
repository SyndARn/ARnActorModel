using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{
    public class DictionaryBehavior<TKey, TValue> : Behaviors, IDictionaryActor<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _dico = new Dictionary<TKey, TValue>();
        public enum DictionaryBehaviorOrder {clear };

        public DictionaryBehavior()
        {
            Behavior<TKey, TValue> bhv1 = new Behavior<TKey, TValue>((k, v) => _dico[k] = v);
            Behavior<IActor, TKey> bhv2 = new Behavior<IActor, TKey>((a, k) =>
                {
                    bool result = _dico.TryGetValue(k, out TValue v);
                    a.SendMessage(result, k, v);
                });
            Behavior<TKey> bhv3 = new Behavior<TKey>(k => _dico.Remove(k));
            Behavior<IActor> bhv4 = new Behavior<IActor>(a =>
            {
                a.SendMessage(_dico.AsEnumerable<KeyValuePair<TKey, TValue>>());
            });
            Behavior<DictionaryBehaviorOrder> bhv5 = new Behavior<DictionaryBehaviorOrder>
                (
                    o => o == DictionaryBehaviorOrder.clear,
                    o => _dico.Clear() 
                ) ;
                
            AddBehavior(bhv1);
            AddBehavior(bhv2);
            AddBehavior(bhv3);
            AddBehavior(bhv4);
            AddBehavior(bhv5);
        }

        public void AddKeyValue(TKey key, TValue value)
        {
            LinkedActor.SendMessage(key, value);
        }

        public IFuture<bool, TKey, TValue> GetKeyValue(TKey key)
        {
            IFuture<bool, TKey, TValue> future = new Future<bool, TKey, TValue>();
            LinkedActor.SendMessage((IActor)future, key);
            return future;
        }

        public IFuture<IEnumerable<KeyValuePair<TKey, TValue>>> AsEnumerable()
        {
            IFuture<IEnumerable<KeyValuePair<TKey, TValue>>> future = new Future<IEnumerable<KeyValuePair<TKey, TValue>>>();
            LinkedActor.SendMessage(future);
            return future;
        }

        public void RemoveKey(TKey key)
        {
            LinkedActor.SendMessage(key);
        }

        public void Clear()
        {
            LinkedActor.SendMessage(DictionaryBehaviorOrder.clear);
        }
    }

    public class DictionaryActor<TKey, TValue> : BaseActor, IDictionaryActor<TKey, TValue>
    {
        private readonly IDictionaryActor<TKey, TValue> _serviceDictionary;

        public DictionaryActor() : base()
        {
            DictionaryBehavior<TKey, TValue> lServiceDictionary = new DictionaryBehavior<TKey, TValue>();
            _serviceDictionary = lServiceDictionary;
            Become(lServiceDictionary);
        }

        public void AddKeyValue(TKey key, TValue value)
        {
            _serviceDictionary.AddKeyValue(key, value);
        }

        public IFuture<bool, TKey, TValue> GetKeyValue(TKey key)
        {
            return _serviceDictionary.GetKeyValue(key);
        }

        public void RemoveKey(TKey key)
        {
            _serviceDictionary.RemoveKey(key);
        }

        public IFuture<IEnumerable<KeyValuePair<TKey,TValue>>> AsEnumerable()
        {
            return _serviceDictionary.AsEnumerable();
        }

        public void Clear()
        {
            _serviceDictionary.Clear();
        }
    }
}
