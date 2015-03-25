using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    public class ActorConsole : actActor
    {
        public ActorConsole()
        {
            actDirectory.GetDirectory().Register(this, "Console");
            Console.WriteLine("Console Start and autoRegister");
            BecomeMany(new bhvConsole());
        }
    }
}
