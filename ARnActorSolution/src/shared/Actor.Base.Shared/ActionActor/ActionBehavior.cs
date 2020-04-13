using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.Base
{
    public class ActionBehavior : Behavior<Action>
    {
        public ActionBehavior()
        {
            Pattern = DefaultPattern();
            Apply = t => t.Invoke();
        }
    }

    public class ActionBehavior<T> : Behavior<Action<T>, T>
    {
        public ActionBehavior()
        {
            Pattern = DefaultPattern();
            Apply = (a, t) => a.Invoke(t);
        }
    }

    public class ActionBehavior<T1, T2> : Behavior<Action<T1, T2>, T1, T2>
    {
        public ActionBehavior()
        {
            Pattern = DefaultPattern();
            Apply = (a, t1, t2) => a.Invoke(t1, t2);
        }
    }
}
