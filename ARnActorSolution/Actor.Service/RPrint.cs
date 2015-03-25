using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Util;

namespace Actor.Service
{
    public class RPrint : actActor 
    {
        public RPrint()
            : base()
        {
            actDirectory.GetDirectory().Register(this, "RPrint");
            actHostDirectory.Register(this);
            Become(new bhvBehavior<string>(DoRPrint));
        }

        private void DoRPrint(string aString)
        {
            Console.WriteLine("RPRINT " + aString);
        }
    }
}
