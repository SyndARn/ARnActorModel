using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "act")]
    public class actEchoActor<T> : BaseActor
    {
        public actEchoActor(IActor dest, T aT)
        {
            CheckArg.Actor(dest);
            Become(new bhvConsole<string>());
            dest.SendMessage(new Tuple<IActor, T>(this, aT));
        }
    }

    public class actEchoActor : BaseActor
    {
        public actEchoActor(IActor dest, String value)
        {
            if (dest == null) throw new ActorException("Dest can't be null");
            BecomeMany(new bhvConsole());
            dest.SendMessage(new Tuple<IActor, String>(this, value));
        }
    }
}
