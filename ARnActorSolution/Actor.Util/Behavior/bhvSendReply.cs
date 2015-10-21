using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{

    public class bhvSendReply<T> : bhvBehavior<Tuple<IActor,T,IActor>>
    {
        public bhvSendReply(T data, IActor target)
            : base()
        {
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

    public class bhvReplySend<T> : bhvBehavior<Tuple<IActor, T, IActor>>
    {
        public bhvReplySend()
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
