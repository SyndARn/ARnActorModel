using System;
using Actor.Base;
using System.Diagnostics;
using System.Globalization;

namespace Actor.Util
{
    public class PeerNewPeer<TKey, TValue> : Behavior<string, IPeerActor<TKey,TValue>, HashKey>
    {
        public PeerNewPeer() : base()
        {
            this.Pattern = (s, i, h) => s == PeerOrder.PeerNewPeer;
            this.Apply = (s, i, h) =>
            {
                Debug.WriteLine(String.Format(CultureInfo.InvariantCulture,"New peer : {0}", h.ToString()),"PeerBehavior");
                (LinkedTo as PeerBehaviors<TKey, TValue>).Peers[h] = i;
            };
        }
    }

}
