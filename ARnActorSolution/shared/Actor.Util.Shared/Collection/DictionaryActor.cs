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
        private Dictionary<TKey, TValue> fDico = new Dictionary<TKey, TValue>();

        public DictionaryBehavior()
        {
            var bhv1 = new Behavior<TKey, TValue>( (k, v) => fDico[k] = v);
            var bhv2 = new Behavior<IActor, TKey>((a, k) =>
                {
                    TValue v ;
                    bool result = fDico.TryGetValue(k, out v);
                    a.SendMessage(result, k, v);
                });
            AddBehavior(bhv1);
            AddBehavior(bhv2);
        }

        public void AddKeyValue(TKey key, TValue value)
        {
            LinkedActor.SendMessage(key, value);
        }

        public Future<Tuple<bool, TKey, TValue>> GetKeyValue(TKey key)
        {
            var future = new Future<Tuple<bool, TKey, TValue>>();
            LinkedActor.SendMessage(future, key);
            return future;
        }
    }


    public class DictionaryActor<TKey,TValue> : BaseActor, IDictionaryActor<TKey, TValue>
    {
        private IDictionaryActor<TKey, TValue> fServiceDictionary;

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

        public Future<Tuple<bool, TKey, TValue>> GetKeyValue(TKey key)
        {
            return fServiceDictionary.GetKeyValue(key);
        }
    }
}
