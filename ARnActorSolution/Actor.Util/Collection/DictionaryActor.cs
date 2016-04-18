using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{
    public class DictionaryActor<K,V> : BaseActor
    {

        private Dictionary<K, V> fDico = new Dictionary<K, V>();

        public DictionaryActor() : base()
        {
            Become(new Behavior<K, V>(DoAddKV));
            AddBehavior(new Behavior<IActor,K>(DoGetKV));
        }

        public void AddKV(K K, V V)
        {
            this.SendMessage(K, V);
        }

        private void DoAddKV(K k, V v) => fDico[k] = v ;

        public Future<Tuple<bool,K,V>> GetKV(K k)
        {
            var future = new Future<Tuple<bool, K, V>>();
            this.SendMessage(future, k);
            return future;
        }

        private void DoGetKV(IActor actor, K k)
        {
            V v;
            bool result = fDico.TryGetValue(k, out v);
            actor.SendMessage(result, k, v); 
        }

    }
}
