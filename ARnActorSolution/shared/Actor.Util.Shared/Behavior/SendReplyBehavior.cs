using Actor.Base;
using System;

namespace Actor.Util
{

    public class SendReplyBehavior<T> : Behavior<IActor, T, IActor>
    {
        public SendReplyBehavior()
            : base()
        {
            Pattern = DefaultPattern();
            Apply = (sender,t,target) => 
                {
                    sender.SendMessage(Tuple.Create(this,t,target)) ;
                } ;
        }
    }

}
