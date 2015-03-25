using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestActor
{
    public class actTestLauncher : actActionActor
    {
        public actTestLauncher()
            : base()
        {
        }

        public void Finish()
        {
            SendMessageTo(true);
        }

        public bool Wait()
        {
            var val = Receive(t => t is bool);
            return (bool)val.Result;
        }
    }
}
