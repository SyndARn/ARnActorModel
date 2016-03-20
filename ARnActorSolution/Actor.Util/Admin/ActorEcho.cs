using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "act")]
    public class actEchoActor<T> : actActor
    {
        public actEchoActor(IActor dest, T aT)
        {
            Become(new bhvConsole<string>());
            dest.SendMessage(new Tuple<IActor, T>(this, aT));
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "act")]
    public class actEchoActor : actActor
    {
        public actEchoActor(IActor Dest, String value)
        {
            BecomeMany(new bhvConsole());
            Dest.SendMessage(new Tuple<IActor, String>(this, value));
        }
    }
}
