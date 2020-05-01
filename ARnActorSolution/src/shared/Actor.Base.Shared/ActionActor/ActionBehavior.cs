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

        public ActionBehavior(Func<Action,bool> pattern, Action<Action> apply)
        {
            Pattern = pattern;
            Apply = apply;
        }
    }

    public class ActionBehavior<T> : Behavior<Action<T>, T>
    {
        public ActionBehavior()
        {
            Pattern = DefaultPattern();
            Apply = (a, t) => a.Invoke(t);
        }
        public ActionBehavior(Func<Action<T>, T, bool> pattern, Action<Action<T>, T> apply)
        {
            Pattern = pattern;
            Apply = apply;
        }
    }

    public class ActionBehavior<T1, T2> : Behavior<Action<T1, T2>, T1, T2>
    {
        public ActionBehavior()
        {
            Pattern = DefaultPattern();
            Apply = (a, t1, t2) => a.Invoke(t1, t2);
        }
        public ActionBehavior(Func<Action<T1,T2>, T1,T2, bool> pattern, Action<Action<T1,T2>, T1,T2> apply)
        {
            Pattern = pattern;
            Apply = apply;
        }
    }

    public class ActionBehavior<T1, T2, T3> : Behavior<Action<T1, T2, T3>, T1, T2, T3>
    {
        public ActionBehavior()
        {
            Pattern = DefaultPattern();
            Apply = (a, t1, t2, t3) => a.Invoke(t1, t2, t3);
        }
    }
}
