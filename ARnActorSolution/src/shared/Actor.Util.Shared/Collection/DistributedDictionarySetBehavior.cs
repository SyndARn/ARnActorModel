using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;

namespace Actor.Util
{
    public class DistributedDictionarySetBehavior<TKey, TValue> : Behavior<TKey, TValue>
    {
        private DistributedDictionaryBehaviors<TKey, TValue> Parent()
        {
            return this.LinkedTo as DistributedDictionaryBehaviors<TKey, TValue>;
        }
        public DistributedDictionarySetBehavior() : base()
        {
            Pattern = (k, v) => true;
            Apply = (k, v) =>
            {
                var hashActor = Parent().GetHashActor(k);
                hashActor.SendMessage(k, v);
            };
        }
    }
}
