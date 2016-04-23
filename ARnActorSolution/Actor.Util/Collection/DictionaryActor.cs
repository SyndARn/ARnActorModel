using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{

    public class DictionaryBehavior<Key,Value> : Behaviors, IDictionaryActor<Key,Value>
    {
        private Dictionary<Key, Value> fDico = new Dictionary<Key, Value>();

        public DictionaryBehavior()
        {
            var bhv1 = new Behavior<Key, Value>( (k, v) => fDico[k] = v);
            var bhv2 = new Behavior<IActor, Key>((a, k) =>
                {
                    Value v ;
                    bool result = fDico.TryGetValue(k, out v);
                    a.SendMessage(result, k, v);
                });
            AddBehavior(bhv1);
            AddBehavior(bhv2);
        }

        public void AddKV(Key K, Value V)
        {
            LinkedActor.SendMessage(K, V);
        }

        public Future<Tuple<bool, Key, Value>> GetKV(Key k)
        {
            var future = new Future<Tuple<bool, Key, Value>>();
            LinkedActor.SendMessage(future, k);
            return future;
        }
    }


    public class DictionaryActor<K,V> : BaseActor, IDictionaryActor<K, V>
    {
        private IDictionaryActor<K, V> fServiceDictionary;

        public DictionaryActor() : base()
        {
            DictionaryBehavior<K, V> lServiceDictionary = new DictionaryBehavior<K, V>();
            fServiceDictionary = lServiceDictionary;
            BecomeMany(lServiceDictionary);
        }

        public void AddKV(K K, V V)
        {
            fServiceDictionary.AddKV(K, V);
        }

        public Future<Tuple<bool, K, V>> GetKV(K k)
        {
            return fServiceDictionary.GetKV(k);
        }
    }
}
