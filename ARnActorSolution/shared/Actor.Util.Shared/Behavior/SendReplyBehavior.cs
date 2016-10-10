using Actor.Base;
using System;

namespace Actor.Util
{

    public class SendReplyBehavior<T> : Behavior<IActor, T>
    {
        public SendReplyBehavior()
            : base()
        {
            Pattern = DefaultPattern();
            Apply = (sender,t) => 
                {
                    sender.SendMessage((IActor)LinkedActor,t) ;
                } ;
        }
    }

}
