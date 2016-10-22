using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

[assembly: CLSCompliant(true)]
namespace Actor.Util
{
    public class ReceiveActor<T> : BaseActor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public async Task<Tuple<IActor, T>> Wait(IActor target, T question)
        {
            var r = Receive(t => t is Tuple<IActor, T>);
            target.SendMessage((IActor)this, question);
            return (Tuple<IActor, T>)await r;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public async Task<Tuple<IActor, T>> Wait(IActor target, T question, int timeOutMS)
        {
            var r = Receive(t => t is Tuple<IActor, T>, timeOutMS);
            target.SendMessage((IActor)this, question);
            return (Tuple<IActor, T>)await r;
        }
        public void SetResult(IActor sender, T data) => this.SendMessage(sender, data);
    }

    public class ReceiveActor<TQuestion, TAnswer> : BaseActor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public async Task<Tuple<IActor, TAnswer>> Wait(IActor target, TQuestion question)
        {
            var r = Receive(t => t is Tuple<IActor, TAnswer>);
            target.SendMessage((IActor)this, question);
            return (Tuple<IActor, TAnswer>)await r;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public async Task<Tuple<IActor, TAnswer>> Wait(IActor target, TQuestion question, int timeOutMS)
        {
            var r = Receive(t => t is Tuple<IActor, TAnswer>, timeOutMS);
            target.SendMessage((IActor)this, question);
            return (Tuple<IActor, TAnswer>)await r;
        }
        public void SetResult(IActor sender, TAnswer data) => this.SendMessage(sender, data) ;
    }

}
