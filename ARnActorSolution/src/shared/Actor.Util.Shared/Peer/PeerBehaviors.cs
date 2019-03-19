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
                return fCurrentPeer ?? (fCurrentPeer = HashKey.ComputeHash(LinkedActor.Tag.Key()));
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

        public void DeleteNode(TKey key)
        {
            LinkedActor.SendMessage(PeerOrder.PeerDeleteNode, key);
        }

        public void GetNode(TKey key, IActor actor)
        {
            LinkedActor.SendMessage(PeerOrder.PeerGetNode, key, actor);
        }

        public IFuture<TValue> GetNode(TKey key)
        {
            IFuture<TValue> future = new Future<TValue>();
            LinkedActor.SendMessage(PeerOrder.PeerGetNode, key, future);
            return future;
        }

        public void StoreNode(TKey key, TValue value)
        {
            LinkedActor.SendMessage(PeerOrder.PeerStoreNode, key, value);
        }

        public IFuture<HashKey> GetHashKey()
        {
            var future = new Future<HashKey>();
            LinkedActor.SendMessage(future);
            return future;
        }
    }

    public interface IPeerActor<TKey> : IActor
    {
        IFuture<HashKey> GetPeerHashKey();
    }

    public interface IPeerActor<TKey,TValue> :
        IActor,
        IPeerBehavior<TKey,TValue>,
        IAgentBehavior<TKey>,
        INodeBehavior<TKey, TValue>
    { }

    public sealed class PeerActor<TKey, TValue> : BaseActor, IPeerActor<TKey>, IPeerActor<TKey,TValue>, INodeBehavior<TKey, TValue>
    {
        private readonly INodeBehavior<TKey, TValue> _nodeBehaviorService;

        public PeerActor() : base()
        {
            var bhv = new PeerBehaviors<TKey, TValue>();
            _nodeBehaviorService = bhv;
            Become(bhv);
        }

        public void FindPeer(TKey key, IFuture<IPeerActor<TKey,TValue>> actor)
        {
            this.SendMessage(PeerOrder.PeerFindPeer, key, actor);
        }

        public void NewPeer(IPeerActor<TKey,TValue> actor, HashKey hash)
        {
            this.SendMessage(PeerOrder.PeerNewPeer, actor, hash);
        }

        public IFuture<IEnumerable<TKey>> AskKeys()
        {
            var future = new Future<IEnumerable<TKey>>();
            this.SendMessage(PeerOrder.PeerAgentAskKeys,future);
            return future;
        }

        public IFuture<IEnumerable<IPeerActor<TKey>>> AskPeers()
        {
            var future = new Future<IEnumerable<IPeerActor<TKey>>>();
            this.SendMessage(PeerOrder.PeerAgentAskNodes,future);
            return future;
        }

        public void StoreNode(TKey key, TValue value)
        {
            _nodeBehaviorService.StoreNode(key, value);
        }

        public IFuture<TValue> GetNode(TKey key)
        {
            return _nodeBehaviorService.GetNode(key);
        }

        public void DeleteNode(TKey key)
        {
            _nodeBehaviorService.DeleteNode(key);
        }

        public IFuture<HashKey> GetHashKey()
        {
            return _nodeBehaviorService.GetHashKey();
        }

        IFuture<HashKey> IPeerActor<TKey>.GetPeerHashKey()
        {
            return _nodeBehaviorService.GetHashKey();
        }
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
