using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Util;
using Actor.Server;

namespace Actor.Service
{
    public class RPrint : BaseActor 
    {
        public RPrint()
            : base()
        {
            DirectoryActor.GetDirectory().Register(this, "RPrint");
            HostDirectoryActor.Register(this);
            Become(new Behavior<string>(DoRPrint));
        }

        private void DoRPrint(string aString)
        {
            Console.WriteLine("RPRINT " + aString);
        }
    }
}
