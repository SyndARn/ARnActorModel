using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;
using System.Linq;

namespace Actor.Util
{

    public class DistributedDictionaryBehaviors<K, V> : Behaviors
    {
        internal Dictionary<int,HashActor<K, V>> HashActorList = new Dictionary<int,HashActor<K, V>>();
        public DistributedDictionaryBehaviors() : base()
        {
            BecomeBehavior(new DistributedDictionaryGetBehavior<K, V>());
            AddBehavior(new DistributedDictionarySetBehavior<K, V>());
        }
    }

    public class KVActor<K, V> : BaseActor
    {
        K key;
        V value;
        public KVActor() : base()
        {
            Become(new Behavior<K, V>((k, v) =>
            {
                key = k;
                value = v;
            }
            ));
            AddBehavior(new Behavior<IActor, K>(
             (i, k) =>
                {
                    i.SendMessage(k);
                }));
        }
    }

    public class HashActor<K, V> : BaseActor
    {
        private string HashKey;
        private List<K> KeyList = new List<K>();
        public HashActor() : base()
        {
            Become(new Behavior<string>(s =>
            {
                HashKey = s;
                Become(new Behavior<K>(k =>
                {
                    KeyList.Add(k);
                }));
                AddBehavior(new Behavior<IActor, K>((i, k) =>
                 {
                     i.SendMessage(k);
                 }
                ));
            }));
        }
    }

    public class DistributedDictionaryGetHashActor<K> : Behavior<IActor, K>
    {
        public DistributedDictionaryGetHashActor() : base()
        {
            Pattern = (i, k) => true;
            Apply = (i, k) => { }; // found hash actor in hash actor list or cast one, send back
        }
    }

    public class DistributedDictionarySetBehavior<K, V> : Behavior<K, V>
    {
        private DistributedDictionaryBehaviors<K, V> parent()
        {
            return this.LinkedTo as DistributedDictionaryBehaviors<K, V>;
        }
        public DistributedDictionarySetBehavior() : base()
        {
            Pattern = (k, v) => true;
            Apply = (k, v) => 
            {
                var hashKey = k.GetHashCode();
                HashActor<K, V> hashActor = null;
                if (parent().HashActorList.TryGetValue(hashKey,out hashActor))
                    {
                    hashActor.SendMessage(k,v);
                    }
            }; // calc hash for key, found hash actor, store key
        }
    }

    public class DistributedDictionaryGetBehavior<K, V> : Behavior<IActor, V>
    {
        public DistributedDictionaryGetBehavior() : base()
        {
            Pattern = (IActor, K) => true;
            Apply = (IActor, K) => { }; // calc hash for key, found hash actor, send actor to actor key
        }
    }
}
