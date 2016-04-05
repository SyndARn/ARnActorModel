using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{
    public class ReceiveActor<T> : BaseActor
    {
        public async Task<Tuple<IActor, T>> Wait(IActor target, T k)
        {
            var r = Receive(t => t is Tuple<IActor, T>);
            target.SendMessage(new Tuple<IActor, T>(this, k));
            return (Tuple<IActor, T>)await r;
        }
        public async Task<Tuple<IActor, T>> Wait(IActor target, T k, int timeOutMs)
        {
            var r = Receive(t => t is Tuple<IActor, T>, timeOutMs);
            target.SendMessage(new Tuple<IActor, T>(this, k));
            return (Tuple<IActor, T>)await r;
        }
    }

    public class ReceiveActor<Q, R> : BaseActor
    {
        public async Task<Tuple<IActor, R>> Wait(IActor target, Q k)
        {
            var r = Receive(t => t is Tuple<IActor, R>);
            target.SendMessage(new Tuple<IActor, Q>(this, k));
            return (Tuple<IActor, R>)await r;
        }
        public async Task<Tuple<IActor, R>> Wait(IActor target, Q k, int timeOutMs)
        {
            var r = Receive(t => t is Tuple<IActor, R>, timeOutMs);
            target.SendMessage(new Tuple<IActor, Q>(this, k));
            return (Tuple<IActor, R>)await r;
        }
    }

}
