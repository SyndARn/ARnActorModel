using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{
    public class DictionaryAssistant<K,V> : BaseActor
    {
        public DictionaryAssistant() : base()
        {

        }

        public async Task<Tuple<bool, K, V>> GetV(DictionaryActor<K, V> IActor, K K)
        {
            var task =Receive(t => t is Tuple<bool, K, V>);
            IActor.SendMessage(new Tuple<IActor, K>(this, K));
            return await task as Tuple<bool, K, V>;
        }

        public void AddV(DictionaryActor<K, V> IActor, K K, V V)
        {
            IActor.SendMessage(new Tuple<K, V>(K, V));
        }
    }

    public class DictionaryActor<K,V> : BaseActor
    {

        private Dictionary<K, V> fDico = new Dictionary<K, V>();

        public DictionaryActor() : base()
        {
            Become(new Behavior<K, V>(AddKV));
            AddBehavior(new Behavior<IActor,K>(GetKV));
        }

        private void AddKV(K k, V v)
        {
            fDico[k] = v ;
        }

        private void GetKV(IActor actor, K k)
        {
            V v;
            bool result = fDico.TryGetValue(k, out v);
            actor.SendMessage(result, k, v); 
        }

    }
}
