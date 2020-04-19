using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;

namespace Actor.Util
{
    public class Work<T> : BaseActor
    {
        private readonly T _t;

        public Work(T t)
        {
            _t = t;
            Become(new Behavior<IActor>(DoIt));
        }

        protected void DoIt(IActor anActor)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(this);
        }
    }
}
