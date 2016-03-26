using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{
    public class OnBehalfActor : BaseActor
    {
        public OnBehalfActor() : base()
        {
            Become(new Behavior<Tuple<Action, IActor>>(DoIt));
        }

        private void DoIt(Tuple<Action, IActor> msg)
        {
            msg.Item2.SendMessage(msg.Item1);
            Become(null);
        }
    }
}
