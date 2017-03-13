using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;
using System.Linq;
using System.Diagnostics;
using System.Globalization;

namespace Actor.Util
{

    internal static class PeerOrder
    {
        public const string PeerDeleteNode = "PeerDeleteNode";
        public const string PeerGetNode = "PeerGetNode";
        public const string PeerNewPeer = "PeerNewPeer";
        public const string PeerStoreNode = "PeerStoreNode";
        public const string PeerFindPeer = "PeerFindPeer";
        public const string PeerAgentAskKeys = "PeerAgentAskKeys";
        public const string PeerAgentAskNodes = "PeerAgentAskNodes";
    }

    public class PeerBehaviors<TKey, TValue> : Behaviors, INodeBehavior<TKey, TValue>
    {
        internal Dictionary<TKey, TValue> Nodes = new Dictionary<TKey, TValue>();
        internal Dictionary<HashKey, IPeerActor<TKey,TValue>> Peers = new Dictionary<HashKey, IPeerActor<TKey,TValue>>();
        private HashKey fCurrentPeer;
        internal HashKey CurrentPeer
        {
            get
            {
                if (fCurrentPeer == null)
                {
                    fCurrentPeer = HashKey.ComputeHash(LinkedActor.Tag.Key());
                }
                return fCurrentPeer;
            }
        }
        public PeerBehaviors() : base()
        {
            BecomeBehavior(new PeerStoreNode<TKey, TValue>())
                .AddBehavior(new PeerGetNode<TKey, TValue>())
                .AddBehavior(new PeerFindPeer<TKey, TValue>())
                .AddBehavior(new PeerNewPeer<TKey, TValue>())
                .AddBehavior(new PeerDeleteNode<TKey, TValue>())
                .AddBehavior(new Behavior<IFuture<HashKey>>(f => f.SendMessage(this.CurrentPeer)))
                .AddBehavior(new Behavior<IFuture<IEnumerable<TKey>>>())// AskKeys();
                .AddBehavior(new Behavior<IFuture<IEnumerable<Tuple<HashKey, IActor>>>>()); // AskNodes();
        }

        public void DeleteNode(K k)
        {
            LinkedActor.SendMessage(PeerOrder.PeerDeleteNode, key);
        }

        public void GetNode(K k, IActor actor)
        {
            LinkedActor.SendMessage(PeerOrder.PeerGetNode, key, actor);
        }

        public IFuture<HashKey> GetHashKey()
        {
            var future = new Future<HashKey>();
            LinkedActor.SendMessage(future);
            return future;
        }

        public void StoreNode(K k, V v)
        {
            LinkedActor.SendMessage(PeerOrder.PeerStoreNode, key, value);
        }

    }

    public class PeerDeleteNode<TKey, TValue> : Behavior<string, TKey>
    {
        public PeerDeleteNode() : base()
        {
            this.Pattern = (s, k) => s == "PeerDeleteNode";
            this.Apply = (s, k) => (LinkedTo as PeerBehaviors<K, V>).Nodes.Remove(k);
        }
    }

    public class PeerStoreNode<TKey, TValue> : Behavior<string, TKey, TValue>
    {
        public PeerStoreNode() : base()
        {
            this.Pattern = (s, k, v) => s == PeerOrder.PeerStoreNode ;
            this.Apply = (s, k, v) =>
            {
                (LinkedTo as PeerBehaviors<TKey, TValue>).Nodes[k] = v;
                Debug.WriteLine(string.Format("New node in : {0}", (LinkedTo as PeerBehaviors<TKey, TValue>).CurrentPeer.ToString()), "PeerBehavior");
            };
        }
    }

    public class PeerGetNode<TKey, TValue> : Behavior<string, TKey, IActor>
    {
        public PeerGetNode() : base()
        {
            this.Pattern = (s, k, i) => s == "PeerGetNode";
            this.Apply = (s, k, i) => i.SendMessage((LinkedTo as PeerBehaviors<K, V>).Nodes[k]);
        }
    }

    public class PeerNewPeer<K, V> : Behavior<string, IPeerActor<K,V>, HashKey>
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
                    i.SendMessage(LinkedActor as IPeerActor<K,V>);
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

    public interface IPeerBehavior<K,V>
    {
        void FindPeer(K k, IFuture<IPeerActor<K,V>> actor);
        void NewPeer(IPeerActor<K,V> actor, HashKey hash);
    }

    public interface INodeBehavior<TKey, TValue>
    {
        void StoreNode(K k, V v);
        void GetNode(K k, IActor actor);
        void DeleteNode(K k);
        IFuture<HashKey> GetHashKey();
    }

    public interface IAgentBehavior<TKey>
    {
        IFuture<IEnumerable<TKey>> AskKeys();
        IFuture<IEnumerable<Tuple<HashKey,IActor>>> AskPeers();
    }

    public interface IPeerActor<K,V> : IActor, IPeerBehavior<K,V>, IAgentBehavior<K>, INodeBehavior<K, V>
    { }

    public class PeerActor<K, V> : BaseActor, IPeerActor<K,V>, INodeBehavior<K, V>
    {
        private INodeBehavior<TKey, TValue> fNodeBehaviorService;
        public PeerActor() : base()
        {
            var bhv = new PeerBehaviors<TKey, TValue>();
            fNodeBehaviorService = bhv;
            Become(bhv);
        }

        public void FindPeer(K k, IFuture<IPeerActor<K,V>> actor)
        {
            this.SendMessage(PeerOrder.PeerFindPeer, key, actor);
        }
        public void NewPeer(IPeerActor<K,V> actor, HashKey hash)
        {
            this.SendMessage(PeerOrder.PeerNewPeer, actor, hash);
        }


        public IFuture<IEnumerable<TKey>> AskKeys()
        {
            var future = new Future<IEnumerable<K>>();
            this.SendMessage("AgentAskKeys",future);
            return future;
        }

        public IFuture<IEnumerable<Tuple<HashKey, IActor>>> AskPeers()
        {
            var future = new Future<IEnumerable<Tuple<HashKey, IActor>>>();
            this.SendMessage(PeerOrder.PeerAgentAskNodes,future);
            return future;
        }

        public void StoreNode(K k, V v)
        {
            fNodeBehaviorService.StoreNode(key, value);
        }

        public void GetNode(K k, IActor actor)
        {
            fNodeBehaviorService.GetNode(key, actor);
        }

        public void DeleteNode(K k)
        {
            fNodeBehaviorService.DeleteNode(key);
        }

        public IFuture<HashKey> GetHashKey()
        {
            return fNodeBehaviorService.GetHashKey();
        }
        // kv behaviors
        // store kv
        // searchK
        // delete kv
        // nodelist
        // receive new node
        // check for node existence

        // using node list order by key
        // k xor on known value >= current hash ==> goto next node
        // else store here

        // new node => add to list
        // agent find abnormal key (not in 3 middle value)
        // go to next node with key
    }

}
