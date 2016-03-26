using Actor.Base;
using System;

[assembly: CLSCompliant(true)]
namespace Actor.Util
{

    public class SendReplyBehavior<T> : Behavior<Tuple<IActor,T,IActor>>
    {
        public SendReplyBehavior(T data, IActor target)
            : base()
        {
            CheckArg.Actor(target);
            Pattern = t => { return t is Tuple<IActor,T,IActor>; };
            Apply = DoReceiveReplyBehavior;
            target.SendMessage(Tuple.Create(this, data, target));
        }

        private void DoReceiveReplyBehavior(Tuple<IActor,T,IActor> msg)
        {
            ReceiveReplyBehavior(msg.Item2);
        }

        public virtual void ReceiveReplyBehavior(T msg)
        {
        }
    }

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
