using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;

namespace Actor.Util
{
    public enum BehaviorMark { Ask, Reached}

    public class MarkBehavior : Behavior<BehaviorMark,IActor>
    {
        public MarkBehavior() : base()
        {
            this.Pattern = (m,a) => { return m == BehaviorMark.Ask; };
            this.Apply = (m, a) => { a.SendMessage(BehaviorMark.Reached); };
        }
    }
}
