using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;

namespace Actor.Util
{
    public class Work<T> : BaseActor
    {
        private readonly T _item;

        public Work(T item)
        {
            _item = item;
            Become(new Behavior<IActor>(DoIt));
        }

        protected void DoIt(IActor anActor)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(this);
        }
    }
}
