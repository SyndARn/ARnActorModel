using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{

    public class Behaviors
    {
        internal List<IBehavior> fList = new List<IBehavior>();
        public actActor LinkedActor { get; private set; }
        public Behaviors()
        {
        }
        public void LinkToActor(actActor anActor)
        {
            LinkedActor = anActor;
        }

        public void AddBehavior(IBehavior aBehavior)
        {
            if (aBehavior != null)
            {
                aBehavior.LinkBehaviors(this);
                fList.Add(aBehavior);
            }
        }
        public void RemoveBehavior(IBehavior aBehavior)
        {
            aBehavior.LinkBehaviors(null);
            fList.Remove(aBehavior);
        }
        public IEnumerable<IBehavior> GetBehaviors()
        {
            return fList ;
        }
        public bool NotEmpty
        {
            get { return fList.Count > 0; }
        }
    }

    // type behavior, type is the acq of the message to be send to this behavior
    public class bhvBehavior<T> : IBehavior<T>, IBehavior 
    {
        public Func<T,Boolean> Pattern { get; protected set; }
        public Action<T> Apply { get; protected set; }
        private Behaviors fLinkedBehaviors;

        public Behaviors LinkedTo()
        {
            return fLinkedBehaviors;
        }

        public void LinkBehaviors(Behaviors someBehaviors)
        {
            fLinkedBehaviors = someBehaviors;
        }

        public bhvBehavior(Func<T, Boolean> aPattern, Action<T> anApply)
        {
            Pattern = aPattern;
            Apply = anApply;
        }
        
        public bhvBehavior()
        {
        }
        
        public void SendMessageTo(Object aData,IActor Target)
        {
            this.LinkedTo().LinkedActor.SendMessageTo(aData,Target);
        }
    
        public bhvBehavior(Action<T> anApply)
        {
            Pattern = t => t is T;
            Apply = anApply;
        }

        public Boolean StandardPattern(Object aT)
        {
            if (Pattern == null)
                return false;
            if (aT is T)
                return Pattern((T)aT);
            else
                return false;
        }

        public void StandardApply(Object aT)
        {
            if (Apply != null)
            {
                Apply((T)aT) ;
            }
        }
    }
}
    
