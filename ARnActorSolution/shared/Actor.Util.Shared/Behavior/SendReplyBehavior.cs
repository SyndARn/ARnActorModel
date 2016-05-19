using Actor.Base;
using System;

namespace Actor.Util
{

    public class ReplySendBehavior<T> : Behavior<Tuple<IActor, T, IActor>>
    {
        public ReplySendBehavior()
            : base()
        {
            Pattern = t => { return t is Tuple<IActor, T, IActor>; };
            Apply = t => 
                {
                    t.Item1.SendMessage(Tuple.Create(this,t.Item2,t.Item1)) ;
                } ;
        }
    }

}
