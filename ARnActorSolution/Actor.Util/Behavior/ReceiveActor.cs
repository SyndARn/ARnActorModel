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
        public async Task<Tuple<IActor, T>> Wait(IActor target, T question)
        {
            var r = Receive(t => t is Tuple<IActor, T>);
            target.SendMessage(new Tuple<IActor, T>(this, question));
            return (Tuple<IActor, T>)await r;
        }
        public async Task<Tuple<IActor, T>> Wait(IActor target, T question, int timeOutMs)
        {
            var r = Receive(t => t is Tuple<IActor, T>, timeOutMs);
            target.SendMessage(new Tuple<IActor, T>(this, question));
            return (Tuple<IActor, T>)await r;
        }

        public void SetResult(IActor sender, T data)
        {
            this.SendMessage(new Tuple<IActor, T>(sender, data));
        }
    }

    public class ReceiveActor<Q, R> : BaseActor
    {
        public async Task<Tuple<IActor, R>> Wait(IActor target, Q question)
        {
            var r = Receive(t => t is Tuple<IActor, R>);
            target.SendMessage(new Tuple<IActor, Q>(this, question));
            return (Tuple<IActor, R>)await r;
        }
        public async Task<Tuple<IActor, R>> Wait(IActor target, Q question, int timeOutMs)
        {
            var r = Receive(t => t is Tuple<IActor, R>, timeOutMs);
            target.SendMessage(new Tuple<IActor, Q>(this, question));
            return (Tuple<IActor, R>)await r;
        }

        public void SetResult(IActor sender, R data)
        {
            this.SendMessage(new Tuple<IActor, R>(sender, data));
        }
    }

}
