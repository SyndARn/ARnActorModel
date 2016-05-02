using Actor.Base;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{

    public class EchoActor<T> : BaseActor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Valider les arguments de méthodes publiques", MessageId = "0")]
        public EchoActor(IActor dest, T aT)
        {
            CheckArg.Actor(dest);
            Become(new ConsoleBehavior<string>());
            dest.SendMessage(new Tuple<IActor, T>(this, aT));
        }
    }

    public class EchoActor : BaseActor
    {
        public EchoActor(IActor dest, String value)
        {
            if (dest == null) throw new ActorException("Dest can't be null");
            Become(new ConsoleBehavior());
            dest.SendMessage(new Tuple<IActor, String>(this, value));
        }
    }
}
