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
        private actCollection<bhvNode<T>> fCollection;

        public bhvGraph() : base()
        {
            fCollection = new actCollection<bhvNode<T>>();
            AddBehavior(new bhvUnLinkNode<bhvNode<T>>());
            AddBehavior(new bhvLinkNode<bhvNode<T>>());
            AddBehavior(new bhvSetNodeValue<Tuple<bhvNode<T>,T>>());
            AddBehavior(new bhvNodeAction<bhvNode<T>>());
        }
    }

    public class bhvNode<T> : bhvStateBehavior<T>
    {
        private actCollection<bhvNode<T>> fLinkedNodes;
        public bhvNode() : base() 
        {
            fLinkedNodes = new actCollection<bhvNode<T>>();
        }
    } ;
    
    public class bhvNodeAction<T> : bhvBehavior<Tuple<bhvNode<T>, T>>
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

    
    public class bhvSetNodeValue<T> : bhvBehavior<Tuple<bhvNode<T>,T>>
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

    public class bhvUnLinkNode<T> : bhvBehavior<Tuple<T, T>>
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
    
    public class bhvLinkNode<T> : bhvBehavior<Tuple<T,T>>
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

    public class actGraph : actActor 
    {
    }
}
