using Actor.Base;

namespace Actor.Util
{
    public class PeerDeleteNode<TKey, TValue> : Behavior<string, TKey>
    {
        public PeerDeleteNode() : base()
        {
            this.Pattern = (s, k) => s == PeerOrder.PeerDeleteNode ;
            this.Apply = (s, k) => (LinkedTo as PeerBehaviors<TKey, TValue>).Nodes.Remove(k);
        }
    }

}
