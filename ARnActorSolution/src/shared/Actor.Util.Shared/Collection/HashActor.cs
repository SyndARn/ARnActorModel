using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;

namespace Actor.Util
{

    public class HashActor<TKey, TValue> : BaseActor
    {
        private Dictionary<TKey, TValue> KeyList = new Dictionary<TKey, TValue>();
        public HashActor() : base()
        {
            Become(new Behavior<TKey, TValue>((k, v) =>
            {
                KeyList[k] = v;
            }));
            AddBehavior(new Behavior<IActor, TKey>((i, k) =>
            {
                bool found = KeyList.TryGetValue(k, out TValue v);
                i.SendMessage(found, k, v);
            }
            ));
        }
    }
}
