using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.TestApplication
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "act")]
    public class actTestLauncher : actActionActor
    {
        public actTestLauncher()
            : base()
        {
        }

        public void Finish()
        {
            SendMessage(true);
        }

        public bool Wait()
        {
            var val = Receive(t => t is bool);
            return (bool)val.Result;
        }
    }
}
