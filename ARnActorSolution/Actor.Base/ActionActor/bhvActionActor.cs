using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base ;

namespace Actor.Base
{

    /// <summary>
    /// bhvAction
    ///     this behavior allows to pass an action as behavior to an actor
    ///     Most frequent use : public method to send an async action to the same actor
    /// </summary>
    public class bhvAction : bhvBehavior<Action>
    {
        public bhvAction()
            : base()
        {
            Pattern = t => { return t is Action ;} ;
            Apply = t => t.Invoke() ;
        }
    }

    public class bhvAction<T> : bhvBehavior<Tuple<Action<T>, T>>
    {
        public bhvAction()
            : base()
        {
            Pattern = t => { return t is Tuple<Action<T>, T> ; };
            Apply = t => { t.Item1.Invoke(t.Item2); };
        }
    }
    
    public class bhvAction<T1,T2> : bhvBehavior<Tuple<Action<T1,T2>, T1,T2>>
    {
        public bhvAction()
            : base()
        {
            Pattern = t => { return t is Tuple<Action<T1,T2>, T1,T2>; };
            Apply = t => { t.Item1.Invoke(t.Item2,t.Item3); };
        }
    }

    /// <summary>
    /// actActionActor
    ///     Action actor are a facility : they provide template to send method as message within an actor
    ///     e.g. SendMessageTo(() => {do something},anActor) ;
    /// </summary>
    public class actActionActor : actActor
    {
        public actActionActor()
            : base()
        {
            Become(new bhvAction());
        }
        public void SendAction(Action anAction)
        {
            SendMessageTo(anAction);
        }
    }

    /// <summary>
    /// actActionActor
    ///   Action actor with the added type parameter if needed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class actAction<T> : actActor
    {
        public actAction()
            : base()
        {
            Behaviors bhvs = new Behaviors();
            bhvs.AddBehavior(new bhvAction());
            bhvs.AddBehavior(new bhvAction<T>());
            BecomeMany(bhvs);
        }

        public void SendAction(Action anAction)
        {
            SendMessageTo(anAction);
        }

        public void SendAction(Action<T> anAction, T aT)
        {
            SendMessageTo(Tuple.Create(anAction, aT));
        }

    }

}
