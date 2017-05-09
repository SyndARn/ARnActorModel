using Actor.Base;

namespace Actor.Util
{
    public class PeerGetNode<TKey, TValue> : Behavior<string, TKey, IActor>
    {
        public PeerGetNode() : base()
        {
            this.Pattern = (s, k, i) => s == PeerOrder.PeerGetNode;
            this.Apply = (s, k, i) => i.SendMessage((LinkedTo as PeerBehaviors<TKey, TValue>).Nodes[k]);
        }
    }

}
