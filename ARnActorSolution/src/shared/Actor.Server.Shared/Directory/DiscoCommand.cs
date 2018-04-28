using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Server
{
    [Serializable]
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
