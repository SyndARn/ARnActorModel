using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{

    public class actEchoActor<T> : actActor
    {
        public actEchoActor(IActor Dest, T aT)
        {
            Become(new bhvConsole<string>());
            SendMessageTo(
                new Tuple<IActor,T>(this, aT), Dest);
        }
    }


        public class ActorEchoActor : actActor
        {
            public ActorEchoActor(IActor Dest, String aString)
            {
                BecomeMany(new bhvConsole());
                SendMessageTo(
                    new Tuple<IActor,String>(this, aString),Dest);
            }
        }
}
