using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    // make it a singleton
    public class ActorConsole : actActor
    {
        public ActorConsole()
        {
            actDirectory.GetDirectory().Register(this, "Console");
            Console.WriteLine("Console starts and autoregisters");
            BecomeMany(new bhvConsole());
        }
    }
}
