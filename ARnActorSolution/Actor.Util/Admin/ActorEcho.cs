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
            Dest.SendMessage(new Tuple<IActor,T>(this, aT));
        }
    }


        public class ActorEchoActor : actActor
        {
            public ActorEchoActor(IActor Dest, String aString)
            {
                BecomeMany(new bhvConsole());
                Dest.SendMessage(new Tuple<IActor,String>(this, aString));
            }
        }
}
