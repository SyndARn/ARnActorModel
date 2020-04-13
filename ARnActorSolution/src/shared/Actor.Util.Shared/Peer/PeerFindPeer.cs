using Actor.Base;
using System.Linq;
using System.Diagnostics;
using System.Globalization;

namespace Actor.Util
{
    public class PeerFindPeer<TKey, TValue> : Behavior<string, TKey, IFuture<IPeerActor<TKey, TValue>>>
    {
        public PeerFindPeer() : base()
        {
            this.Pattern = (s, k, i) => s == PeerOrder.PeerFindPeer;
            this.Apply = (s, k, i) =>
            {
                var key = HashKey.ComputeHash(k.ToString());
                var current = (LinkedTo as PeerBehaviors<TKey, TValue>).CurrentPeer;
                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture,"Search Key {0} in {1}", key.ToString(), current.ToString()) , "PeerBehavior");
                if (key.CompareTo(current) <= 0)
                {
                    // Store here
                    i.SendMessage(LinkedActor as IPeerActor<TKey, TValue>);
                }
                else
                {
                    // find best peer
                    var nextPeer = (LinkedTo as PeerBehaviors<TKey, TValue>).Peers
                      .Where(n => n.Key.CompareTo(current) > 0).OrderBy(n => n.Key).FirstOrDefault();
                    if (nextPeer.Key != null)
                    {
                        nextPeer.Value.SendMessage(s, k, i);
                    }
                    else
                    {
                        i.SendMessage(LinkedActor as IPeerActor<TKey, TValue>);
                    }

                }
            };
        }
    }

}
