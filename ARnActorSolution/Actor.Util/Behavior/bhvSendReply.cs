using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{

    class bhvSendReply<T> : bhvBehavior<Tuple<IActor,T,IActor>>
    {
        public bhvSendReply(T data, IActor target)
            : base()
        {
            Pattern = t => { return t is Tuple<IActor,T,IActor>; };
            Apply = DoReceiveReplyBehavior;
            SendMessageTo(Tuple.Create(this,data,target),target);
        }

        private void DoReceiveReplyBehavior(Tuple<IActor,T,IActor> msg)
        {
            ReceiveReplyBehavior(msg.Item2);
        }

        public virtual void ReceiveReplyBehavior(T msg)
        {
        }
    }

    class bhvReplySend<T> : bhvBehavior<Tuple<IActor, T, IActor>>
    {
        public bhvReplySend()
            : base()
        {
            Pattern = t => { return t is Tuple<IActor, T, IActor>; };
            Apply = t => 
                {
                    SendMessageTo(Tuple.Create(this,t.Item2,t.Item1),t.Item1) ;
                } ;
        }
    }

}
