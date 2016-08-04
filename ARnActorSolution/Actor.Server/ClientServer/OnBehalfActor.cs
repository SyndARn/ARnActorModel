using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Server
{
    public class OnBehalfActor : BaseActor
    {
        public OnBehalfActor() : base()
        {
            Become(new Behavior<Action, IActor>(DoIt));
        }

        private void DoIt(Action action, IActor actor)
        {
            actor.SendMessage(action);
            Become(new NullBehaviors());
        }
    }
}
