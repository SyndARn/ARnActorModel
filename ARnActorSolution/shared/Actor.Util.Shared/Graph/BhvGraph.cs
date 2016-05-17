using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{


    public class BehaviorGraph<N, E> : BaseActor
    {
        private List<NodeActor<N, E>> fNodeCollection;

        public BehaviorGraph() : base()
        {
            fNodeCollection = new List<NodeActor<N, E>>();
            Become(new Behavior<GraphOperation, NodeActor<N, E>>(
                (o, n) => o == GraphOperation.AddNode,
                (o, n) => fNodeCollection.Add(n)));
            AddBehavior(new Behavior<GraphOperation, NodeActor<N, E>>(
                (o, n) => o == GraphOperation.RemoveNode,
                (o, n) => fNodeCollection.Remove(n)));
            AddBehavior(new Behavior<GraphOperation, IActor>(
                (o, a) => o == GraphOperation.PickUpNode,
                (o, a) => {
                    var node = fNodeCollection.FirstOrDefault();
                    a.SendMessage(new Tuple<IActor, NodeActor<N, E>>(this, node));
                }));
        }

        public void AddNode(NodeActor<N, E> node)
        {
            SendMessage(new Tuple<GraphOperation, NodeActor<N, E>>(GraphOperation.AddNode, node));
        }
        public void RemoveNode(NodeActor<N, E> node)
        {
            SendMessage(new Tuple<GraphOperation, NodeActor<N, E>>(GraphOperation.RemoveNode, node));
        }
    }


    /*
    The basic operations provided by a graph data structure G usually include:[1]
adjacent(G, x, y): tests whether there is an edge from the vertices x to y;
neighbors(G, x): lists all vertices y such that there is an edge from the vertices x to y;
add_vertex(G, x): adds the vertex x, if it is not there;
remove_vertex(G, x): removes the vertex x, if it is there;
add_edge(G, x, y): adds the edge from the vertices x to y, if it is not there;
remove_edge(G, x, y): removes the edge from the vertices x to y, if it is there;
get_vertex_value(G, x): returns the value associated with the vertex x;
set_vertex_value(G, x, v): sets the value associated with the vertex x to v.

Structures that associate values to the edges usually also provide:[1]
get_edge_value(G, x, y): returns the value associated with the edge (x, y);
set_edge_value(G, x, y, v): sets the value associated with the edge (x, y) to v.

    */
    public class EdgeActor<N, E> : BaseActor
    {
        private NodeActor<N, E> NodeA;
        private NodeActor<N, E> NodeB;
        private E fData;
        public EdgeActor() : base()
        {
        }
        public EdgeActor(NodeActor<N, E> nodeA, NodeActor<N, E> nodeB)
        {
            NodeA = nodeA;
            NodeB = nodeB;
        }
        private void SetUpBehavior()
        {
            Become(new Behavior<GraphOperation, IActor>(
                (o, a) => o == GraphOperation.GetEdgeValue,
                (o, a) =>
                    {
                    a.SendMessage(new Tuple<IActor, E>(this, fData));
                    }));
            Become(new Behavior<GraphOperation, E>(
                (o, e) => o == GraphOperation.SetEdgeValue,
                (o, e) =>
                {
                    fData = e;
                }));

        }
    }

    public class NodeActor<N, E> : BaseActor
    {
        private Dictionary<NodeActor<N, E>, EdgeActor<N, E>> Links;
        private N fData;
        public NodeActor() : base()
        {
            Links = new Dictionary<NodeActor<N, E>, EdgeActor<N, E>>();
            Become(new Behavior<GraphOperation, NodeActor<N, E>>(
                (o, n) => o == GraphOperation.AddEdge,
                (o, n) =>
                {
                    Links[n] = new EdgeActor<N, E>(this, n);
                }));
            AddBehavior(new Behavior<GraphOperation, NodeActor<N, E>>(
                (o, n) => o == GraphOperation.RemoveEdge,
                (o, n) =>
                {
                    Links.Remove(n);
                }));
            AddBehavior(new Behavior<GraphOperation, IActor>(
                (o, n) => o == GraphOperation.GetNodeValue,
                (o, n) =>
                {
                    n.SendMessage(new Tuple<IActor, N>(this, fData));
                }));
            AddBehavior(new Behavior<GraphOperation, N>(
                (o, n) => o == GraphOperation.SetNodeValue,
                (o, n) =>
                {
                    fData = n;
                }));
            AddBehavior(new Behavior<GraphOperation, NodeActor<N, E>, IActor>(
                (o, n, a) => o == GraphOperation.Adjacent,
                (o, n, a) =>
                {
                    a.SendMessage(new Tuple<IActor, bool>(this, Links.ContainsKey(n)));
                }));
            AddBehavior(new Behavior<GraphOperation, IActor>(
                (o, a) => o == GraphOperation.Neighbors,
                (o, a) =>
                {
                    var list = Links.Select(t => t.Key);
                    a.SendMessage(new Tuple<IActor, IEnumerable<NodeActor<N, E>>>(this, list));
                }));
        }

        public void AddEdge(NodeActor<N,E> node)
        {
            SendMessage(new Tuple<GraphOperation, NodeActor<N, E>>(GraphOperation.AddEdge, node));
        }

        public void RemoveEdge(NodeActor<N, E> node)
        {
            SendMessage(new Tuple<GraphOperation, NodeActor<N, E>>(GraphOperation.RemoveEdge, node));
        }

        public void Adjacent(NodeActor<N,E> nodeA, IActor sender)
        {
            SendMessage(new Tuple<GraphOperation, NodeActor<N, E>, IActor>(GraphOperation.Adjacent, nodeA, sender));
        }

        public void Neighbors(IActor sender)
        {
            SendMessage(new Tuple<GraphOperation, IActor>(GraphOperation.Neighbors, sender));
        }

        public Future<Tuple<IActor, IEnumerable<NodeActor<N, E>>>> Neighbors()
        {
            var future = new Future<Tuple<IActor, IEnumerable<NodeActor<N, E>>>>();
            SendMessage(new Tuple<GraphOperation, IActor>(GraphOperation.Neighbors, future));
            return future;
        }
    }

    public enum GraphOperation
    {
        None, AddNode, RemoveNode, AddEdge, RemoveEdge, GetNodeValue, SetNodeValue, GetEdgeValue, SetEdgeValue,
        Adjacent, Neighbors, PickUpNode
    }

}
