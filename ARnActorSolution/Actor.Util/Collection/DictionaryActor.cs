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

        public async Task<Tuple<bool,K,V>> GetValue(DictionaryActor<K, V> IActor, K key)
        {
            var task =Receive(t => t is Tuple<bool, K, V>);
            IActor.SendMessage(new Tuple<IActor, K>(this, key));
            return await task as Tuple<bool, K, V>;
        }

        public void AddValue(DictionaryActor<K,V> IActor, K key, V value)
        {
            IActor.SendMessage(new Tuple<K, V>(key, value));
        }
    }

    public class DictionaryActor<K,V> : BaseActor
    {

        private Dictionary<K, V> fDico = new Dictionary<K, V>();

        public DictionaryActor() : base()
        {
            Become(new Behavior<Tuple<K, V>>(AddKeyValue));
            AddBehavior(new Behavior<Tuple<IActor,K>>(GetKeyValue));
        }

        private void AddKeyValue(Tuple<K,V> message)
        {
            fDico[message.Item1] =  message.Item2 ;
        }

        private void GetKeyValue(Tuple<IActor,K> message)
        {
            V value;
            bool result = fDico.TryGetValue(message.Item2, out value);
            message.Item1.SendMessage(new Tuple<bool, K, V>(result, message.Item2, value));
        }

    }
}
