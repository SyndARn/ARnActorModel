using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    public class DiscoCommand
    {
        public IActor Sender { get; set; }
        public DiscoCommand() { }
        public DiscoCommand(IActor anActor)
        {
            Sender = anActor;
        }
    }
}
