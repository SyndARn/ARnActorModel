using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{

    public interface IBehavior 
    {
        //Func<Object, Boolean> StdPattern { get; }
        //Action<Object> StdApply { get; }
        void LinkBehaviors(Behaviors someBehavior);
        Behaviors LinkedTo();
        void StandardApply(Object aT);
        bool StandardPattern(Object aT);
    }
    public interface IBehavior<T> : IBehavior 
    {
        Func<T,Boolean> Pattern { get; }
        Action<T> Apply { get; }
    }


}
