using Actor.Base;
using System.Diagnostics;
using System.Globalization;

namespace Actor.Util
{
    public class PeerStoreNode<TKey, TValue> : Behavior<string, TKey, TValue>
    {
        public PeerStoreNode() : base()
        {
            this.Pattern = (s, k, v) => s == PeerOrder.PeerStoreNode ;
            this.Apply = (s, k, v) =>
            {
                (LinkedTo as PeerBehaviors<TKey, TValue>).Nodes[k] = v;
                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture,"New node in : {0}", (LinkedTo as PeerBehaviors<TKey, TValue>).CurrentPeer.ToString()), "PeerBehavior");
            };
        }
    }

}
