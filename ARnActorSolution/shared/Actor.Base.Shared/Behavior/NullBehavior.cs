using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    public class NullBehavior : Behavior<Object>
    {
        public NullBehavior() : base()
        {
            Pattern = t => true;
            Apply = t => { };
        }
    }

    public class NullBehaviors : Behaviors
    {
        public NullBehaviors() : base()
        {
            AddBehavior(new NullBehavior());
        }
    }
}
