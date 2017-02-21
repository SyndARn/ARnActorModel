﻿using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;
using System.Linq;

namespace Actor.Util
{

    public class DistributedDictionaryActor<TKey,TValue> : BaseActor, IDictionaryActor<TKey, TValue>
    {
        public DistributedDictionaryActor() : base()
        {
            Become(new DistributedDictionaryBehaviors<TKey, TValue>());
        }

        public void AddKeyValue(TKey key, TValue value)
        {
            this.SendMessage(key, value);
        }

        public Future<bool, TKey, TValue> GetKeyValue(TKey key)
        {
            var future = new Future<bool, TKey, TValue>();
            this.SendMessage(future, key);
            return future;
        }

        public void RemoveKey(TKey key)
        {
            this.SendMessage(key);
        }
    }

    public class DistributedDictionaryBehaviors<TKey, TValue> : Behaviors
    {
        private Dictionary<int,HashActor<TKey, TValue>> HashActorList = new Dictionary<int,HashActor<TKey, TValue>>();
        public DistributedDictionaryBehaviors() : base()
        {
            BecomeBehavior(new DistributedDictionaryGetBehavior<TKey, TValue>());
            AddBehavior(new DistributedDictionarySetBehavior<TKey, TValue>());
            AddBehavior(new DistributedDictionaryDeleteBehavior<TKey,TValue>());
        }
        internal HashActor<TKey,TValue> GetHashActor(TKey key)
        {
            var hashKey = key.GetHashCode();
            if (HashActorList.ContainsKey(hashKey))
            {
                return HashActorList[hashKey];
            }
            HashActor<TKey, TValue> hashActor = new HashActor<TKey, TValue>();
            HashActorList[hashKey] = hashActor;
            return hashActor;
        }
    }

    public class DistributedDictionaryDeleteBehavior<TKey,TValue> : Behavior<TKey>
    {
        private DistributedDictionaryBehaviors<TKey,TValue> parent()
        {
            return this.LinkedTo as DistributedDictionaryBehaviors<TKey, TValue>;
        }
        public DistributedDictionaryDeleteBehavior() : base()
        {
            Pattern = (k) => true;
            Apply = (k) =>
            {
                var hashActor = parent().GetHashActor(k);
                hashActor.SendMessage(k);
            };
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
                var hashActor = parent().GetHashActor(k);
                hashActor.SendMessage(k, v);
            }; 
        }
    }

    public class DistributedDictionaryGetBehavior<TKey, TValue> : Behavior<IActor, TKey>
    {
        private DistributedDictionaryBehaviors<TKey, TValue> parent()
        {
            return this.LinkedTo as DistributedDictionaryBehaviors<TKey, TValue>;
        }
        public DistributedDictionaryGetBehavior() : base()
        {
            Pattern = (IActor, k) => true;
            Apply = (IActor, k) => 
            {
                var hashActor = parent().GetHashActor(k);
                hashActor.SendMessage(IActor,k);
            }; 
        }
    }

    public class HashActor<TKey, TValue> : BaseActor
    {
        private Dictionary<TKey,TValue> KeyList = new Dictionary<TKey,TValue>();
        public HashActor() : base()
        {
                Become(new Behavior<TKey,TValue>((k,v) =>
                {
                    KeyList[k] = v;
                }));
                AddBehavior(new Behavior<IActor, TKey>((i, k) =>
                 {
                     TValue v;
                     bool found = KeyList.TryGetValue(k, out v);
                     i.SendMessage(found,k,v);
                 }
                ));
        }
    }

}
