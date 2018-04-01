using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.Base
{

    /// <summary>
    /// bhvAction
    ///     this behavior allows to pass an action as behavior to an actor
    ///     Most frequent use : public method to send an async action to the same actor
    /// </summary>
    public class ActionBehavior : Behavior<Action>
    {
        public ActionBehavior()
            : base()
        {
            Pattern = DefaultPattern();
            Apply = t => t.Invoke();
        }
    }

    public class ActionBehavior<T> : Behavior<Action<T>, T>
    {
        public ActionBehavior()
            : base()
        {
            Pattern = DefaultPattern();
            Apply = (a, t) => { a.Invoke(t); };
        }
    }

    public class ActionBehavior<T1, T2> : Behavior<Action<T1, T2>, T1, T2>
    {
        public ActionBehavior()
            : base()
        {
            Pattern = DefaultPattern();
            Apply = (a, t1, t2) => { a.Invoke(t1, t2); };
        }
    }

    public class ActionBehaviors<T> : Behaviors
    {
        public ActionBehaviors() : base()
        {
            AddBehavior(new ActionBehavior());
            AddBehavior(new ActionBehavior<T>());
        }
    }

    public class ActionBehaviors<T1, T2> : Behaviors
    {
        public ActionBehaviors() : base()
        {
            AddBehavior(new ActionBehavior());
            AddBehavior(new ActionBehavior<T1, T2>());
        }
    }
}
