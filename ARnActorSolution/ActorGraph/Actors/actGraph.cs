using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base ;
using Actor.Util ;

namespace ActorGraph.Actors
{

    internal class bhvGraph<T> : Behaviors
    {
        private CollectionActor<bhvNode<T>> fCollection;

        public bhvGraph() : base()
        {
            fCollection = new CollectionActor<bhvNode<T>>();
            AddBehavior(new bhvUnLinkNode<bhvNode<T>>());
            AddBehavior(new bhvLinkNode<bhvNode<T>>());
            AddBehavior(new bhvSetNodeValue<Tuple<bhvNode<T>,T>>());
            AddBehavior(new bhvNodeAction<bhvNode<T>>());
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "bhv")]
    public class bhvNode<T> : StateBehavior<T>
    {
        private CollectionActor<bhvNode<T>> fLinkedNodes;
        public bhvNode() : base() 
        {
            fLinkedNodes = new CollectionActor<bhvNode<T>>();
        }
    } ;
    
    public class bhvNodeAction<T> : Behavior<Tuple<bhvNode<T>, T>>
    {
        public bhvNodeAction()
            : base()
        {
            Pattern = t => { return t is Tuple<bhvNode<T>, T>; };
            Apply = Do;
        }
        private void Do(Tuple<bhvNode<T>, T> msg)
        {
            // do something
        }
    }

    
    public class bhvSetNodeValue<T> : Behavior<Tuple<bhvNode<T>,T>>
    {
        public bhvSetNodeValue()
            : base()
        {
            Pattern = t => { return t is Tuple<bhvNode<T>, T>; };
            Apply = Do;
        }
        private void Do(Tuple<bhvNode<T>, T> msg)
        {
            msg.Item1.SetValue(msg.Item2);
        }
    }

    public class bhvUnLinkNode<T> : Behavior<Tuple<T, T>>
    {
        public bhvUnLinkNode()
            : base()
        {
            Pattern = t => { return t is Tuple<T, T>; };
            Apply = DoIt;
        }

        protected void DoIt(Tuple<T, T> at)
        {
            // get behavior state
            // Unlink the two node
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "bhv")]
    public class bhvLinkNode<T> : Behavior<Tuple<T,T>>
    {
        public bhvLinkNode() : base()
        {
            Pattern = t => { return t is Tuple<T, T>; };
            Apply = DoIt;
        }

        protected void DoIt(Tuple<T, T> at)
        {
            // get behavior state
            // link the two node
        }
    }

    public class actGraph : BaseActor 
    {
    }
}
