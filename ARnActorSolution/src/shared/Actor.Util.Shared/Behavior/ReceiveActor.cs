﻿using System;
using System.Threading.Tasks;
using Actor.Base;

[assembly: CLSCompliant(true)]
namespace Actor.Util
{
    public class ReceiveActor<T> : BaseActor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public async Task<IMessageParam<IActor, T>> WaitAsync(IActor target, T question)
        {
            target.SendMessage((IActor)this, question);
            Task<object> r = ReceiveAsync(t => t is IMessageParam<IActor, T>);
            return (IMessageParam<IActor, T>)await r.ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public async Task<IMessageParam<IActor, T>> WaitAsync(IActor target, T question, int timeOutMS)
        {
            target.SendMessage((IActor)this, question);
            Task<object> r = ReceiveAsync(t => t is IMessageParam<IActor, T>, timeOutMS);
            return (IMessageParam<IActor, T>)await r.ConfigureAwait(false);
        }

        public void SetResult(IActor sender, T data) => this.SendMessage(sender, data);
    }

    public class ReceiveActor<TQuestion, TAnswer> : BaseActor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public async Task<IMessageParam<IActor, TAnswer>> WaitAsync(IActor target, TQuestion question)
        {
            target.SendMessage((IActor)this, question);
            Task<object> r = ReceiveAsync(t => t is IMessageParam<IActor, TAnswer>);
            return (IMessageParam<IActor, TAnswer>)await r.ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public async Task<IMessageParam<IActor, TAnswer>> WaitAsync(IActor target, TQuestion question, int timeOutMS)
        {
            target.SendMessage((IActor)this, question);
            Task<object> r = ReceiveAsync(t => t is IMessageParam<IActor, TAnswer>, timeOutMS);
            return (IMessageParam<IActor, TAnswer>)await r.ConfigureAwait(false);
        }

        public void SetResult(IActor sender, TAnswer data) => this.SendMessage(sender, data) ;
    }
}
