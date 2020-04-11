using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{

    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SupervisedBehavior : Behavior<SupervisorAction>
    {
        public SupervisedBehavior()
        {
            Pattern = t => true;
            Apply = t =>
             {
                 if (t.Equals(SupervisorAction.Kill))
                 {
                     LinkedActor.SendMessage(SystemMessage.NullBehavior);
                 }
             };
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                return ToString();
            }
        }
    }
}
