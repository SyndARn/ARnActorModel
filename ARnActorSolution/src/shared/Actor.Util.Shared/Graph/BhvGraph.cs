using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{
    public class BehaviorGraph<TNode, TEdge> : BaseActor
    {
        private readonly List<NodeActor<TNode, TEdge>> fNodeCollection;

        public BehaviorGraph() : base()
        {
            fNodeCollection = new List<NodeActor<TNode, TEdge>>();
            Become(new Behavior<GraphOperation, NodeActor<TNode, TEdge>>(
                (o, n) => o == GraphOperation.AddNode,
                (o, n) => fNodeCollection.Add(n)));
            AddBehavior(new Behavior<GraphOperation, NodeActor<TNode, TEdge>>(
                (o, n) => o == GraphOperation.RemoveNode,
                (o, n) => fNodeCollection.Remove(n)));
            AddBehavior(new Behavior<GraphOperation, IActor>(
                (o, a) => o == GraphOperation.PickUpNode,
                (o, a) => {
                    NodeActor<TNode, TEdge> node = fNodeCollection.FirstOrDefault();
                    a.SendMessage(this, node);
                }));
        }

        public void AddNode(NodeActor<TNode, TEdge> node) => this.SendMessage(GraphOperation.AddNode, node);

        public void RemoveNode(NodeActor<TNode, TEdge> node) => this.SendMessage(GraphOperation.RemoveNode, node);
    }

    public class EdgeActor<TNode, TEdge> : BaseActor
    {
        private readonly NodeActor<TNode, TEdge> NodeA;
        private readonly NodeActor<TNode, TEdge> NodeB;
        private TEdge fData;

        public EdgeActor() : base()
        {
        }

        public EdgeActor(NodeActor<TNode, TEdge> nodeA, NodeActor<TNode, TEdge> nodeB)
        {
            NodeA = nodeA;
            NodeB = nodeB;
        }

        private void SetUpBehavior()
        {
            Become(new Behavior<GraphOperation, IActor>(
                (o, a) => o == GraphOperation.GetEdgeValue,
                (o, a) => a.SendMessage(this, fData)));
            Become(new Behavior<GraphOperation, TEdge>(
                (o, e) => o == GraphOperation.SetEdgeValue,
                (o, e) => fData = e));
        }
    }

    public class NodeActor<TNode, TEdge> : BaseActor
    {
        private readonly Dictionary<NodeActor<TNode, TEdge>, EdgeActor<TNode, TEdge>> _links;
        private TNode _data;

        public NodeActor() : base()
        {
            _links = new Dictionary<NodeActor<TNode, TEdge>, EdgeActor<TNode, TEdge>>();
            Become(new Behavior<GraphOperation, NodeActor<TNode, TEdge>>(
                (o, n) => o == GraphOperation.AddEdge,
                (o, n) => _links[n] = new EdgeActor<TNode, TEdge>(this, n)));
            AddBehavior(new Behavior<GraphOperation, NodeActor<TNode, TEdge>>(
                (o, n) => o == GraphOperation.RemoveEdge,
                (o, n) => _links.Remove(n)));
            AddBehavior(new Behavior<GraphOperation, IActor>(
                (o, n) => o == GraphOperation.GetNodeValue,
                (o, n) => n.SendMessage(this, _data)));
            AddBehavior(new Behavior<GraphOperation, TNode>(
                (o, n) => o == GraphOperation.SetNodeValue,
                (o, n) => _data = n));
            AddBehavior(new Behavior<GraphOperation, NodeActor<TNode, TEdge>, IActor>(
                (o, n, a) => o == GraphOperation.Adjacent,
                (o, n, a) => a.SendMessage(this, _links.ContainsKey(n))));
            AddBehavior(new Behavior<GraphOperation, IActor>(
                (o, a) => o == GraphOperation.Neighbors,
                (o, a) =>
                {
                    IEnumerable<NodeActor<TNode, TEdge>> list = _links.Select(t => t.Key);
                    a.SendMessage(this, list);
                }));
        }

        public void AddEdge(NodeActor<TNode, TEdge> node) => this.SendMessage(GraphOperation.AddEdge, node);

        public void RemoveEdge(NodeActor<TNode, TEdge> node) => this.SendMessage(GraphOperation.RemoveEdge, node);

        public void Adjacent(NodeActor<TNode, TEdge> nodeA, IActor sender) => this.SendMessage(GraphOperation.Adjacent, nodeA, sender);

        public void Neighbors(IActor sender) => this.SendMessage(GraphOperation.Neighbors, sender);

        public IFuture<IActor, IEnumerable<NodeActor<TNode, TEdge>>> Neighbors()
        {
            Future<IActor, IEnumerable<NodeActor<TNode, TEdge>>> future = new Future<IActor, IEnumerable<NodeActor<TNode, TEdge>>>();
            this.SendMessage(GraphOperation.Neighbors, future);
            return future;
        }
    }
}
