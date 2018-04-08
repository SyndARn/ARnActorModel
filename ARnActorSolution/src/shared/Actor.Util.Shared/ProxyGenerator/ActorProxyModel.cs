using Actor.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.Util
{
    public class ActorProxyModel<T> : BaseActor
        where T : class
    {
        private T fT;
        public ActorProxyModel() : base()
        {
        }
        internal void SetObject(T aT)
        {
            fT = aT;
        }
        internal void AddBehaviors(Behaviors behaviors)
        {
            Become(behaviors);
        }
        public void SendMethodAndParam(string methodName, object param)
        {
            this.SendMessage(methodName, param);
        }
    }
}
