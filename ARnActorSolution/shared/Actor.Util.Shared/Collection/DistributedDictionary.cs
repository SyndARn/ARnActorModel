using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;
using System.Linq;

namespace Actor.Util
{

    public class DistributedDictionaryBehaviors<TKey, TValue> : Behaviors
    {
        internal Dictionary<int,HashActor<TKey, TValue>> HashActorList = new Dictionary<int,HashActor<TKey, TValue>>();
        public DistributedDictionaryBehaviors() : base()
        {
            BecomeBehavior(new DistributedDictionaryGetBehavior<TKey, TValue>());
            AddBehavior(new DistributedDictionarySetBehavior<TKey, TValue>());
        }
    }

    public class KVActor<TKey, TValue> : BaseActor
    {
        TKey key;
        TValue value;
        public KVActor() : base()
        {
            Become(new Behavior<TKey, TValue>((k, v) =>
            {
                key = k;
                value = v;
            }
            ));
            AddBehavior(new Behavior<IActor, TKey>(
             (i, k) =>
                {
                    i.SendMessage(k);
                }));
        }
    }

    public class HashActor<TKey, TValue> : BaseActor
    {
        private string HashKey;
        private List<TKey> KeyList = new List<TKey>();
        public HashActor() : base()
        {
            Become(new Behavior<string>(s =>
            {
                HashKey = s;
                Become(new Behavior<TKey>(k =>
                {
                    KeyList.Add(k);
                }));
                AddBehavior(new Behavior<IActor, TKey>((i, k) =>
                 {
                     i.SendMessage(k);
                 }
                ));
            }));
        }
    }

    public class DistributedDictionaryGetHashActor<TKey> : Behavior<IActor, TKey>
    {
        public DistributedDictionaryGetHashActor() : base()
        {
            Pattern = (i, k) => true;
            Apply = (i, k) => { }; // found hash actor in hash actor list or cast one, send back
        }
    }

    public class DistributedDictionarySetBehavior<TKey, TValue> : Behavior<TKey, TValue>
    {
        private DistributedDictionaryBehaviors<TKey, TValue> parent()
        {
            return this.LinkedTo as DistributedDictionaryBehaviors<TKey, TValue>;
        }
        public DistributedDictionarySetBehavior() : base()
        {
            Pattern = (k, v) => true;
            Apply = (k, v) => 
            {
                var hashKey = k.GetHashCode();
                HashActor<TKey, TValue> hashActor = null;
                if (parent().HashActorList.TryGetValue(hashKey,out hashActor))
                    {
                    hashActor.SendMessage(k,v);
                    }
            }; // calc hash for key, found hash actor, store key
        }
    }

    public class DistributedDictionaryGetBehavior<TKey, TValue> : Behavior<IActor, TValue>
    {
        public DistributedDictionaryGetBehavior() : base()
        {
            Pattern = (IActor, K) => true;
            Apply = (IActor, K) => { }; // calc hash for key, found hash actor, send actor to actor key
        }
    }
}
