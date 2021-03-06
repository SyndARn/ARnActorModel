﻿using System;
using System.Threading.Tasks;

namespace Actor.Base
{
    public class Future<T> : BaseActor, IFuture<T>
    {
        public Future()
        {
        }

        public async Task<object> GetResultAsync() => await ResultAsync().ConfigureAwait(false);

        public T Result() => (T)ReceiveAsync(t => t is T).Result;

        public T Result(int timeOutMS) => (T)ReceiveAsync(t => t is T, timeOutMS).Result;

        public async Task<T> ResultAsync() => (T)await ReceiveAsync(t => t is T).ConfigureAwait(false);

        public async Task<T> ResultAsync(int timeOutMS) => (T)await ReceiveAsync(t => t is T, timeOutMS).ConfigureAwait(false);

        public T Result(Func<object, bool> aPattern) => (T)ReceiveAsync(aPattern).Result;

        public async Task<T> ResultAsync(Func<object, bool> aPattern) => (T)await ReceiveAsync(aPattern).ConfigureAwait(false);

        public async Task<T> ResultAsync(Func<object, bool> aPattern, int timeOutMS) => (T)await ReceiveAsync(aPattern, timeOutMS).ConfigureAwait(false);
    }

    public class Future<T1,T2> : BaseActor, IFuture<T1, T2>
    {
        public Future()
        {
        }

        public async Task<object> GetResultAsync() => await ResultAsync().ConfigureAwait(false);

        public IMessageParam<T1,T2> Result() => (IMessageParam<T1, T2>)ReceiveAsync(t => t is IMessageParam<T1, T2>).Result;

        public IMessageParam<T1, T2> Result(int timeOutMS) => (IMessageParam<T1, T2>)ReceiveAsync(t => t is IMessageParam<T1, T2>, timeOutMS).Result;

        public async Task<IMessageParam<T1, T2>> ResultAsync() => (IMessageParam<T1, T2>)await ReceiveAsync(t => t is IMessageParam<T1, T2>).ConfigureAwait(false);

        public async Task<IMessageParam<T1, T2>> ResultAsync(int timeOutMS) => (IMessageParam<T1, T2>)await ReceiveAsync(t => t is IMessageParam<T1, T2>, timeOutMS).ConfigureAwait(false);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public class Future<T1, T2, T3> : BaseActor, IFuture<T1, T2, T3>
    {
        public Future()
        {
        }

        public async Task<object> GetResultAsync() => await ResultAsync().ConfigureAwait(false);

        public IMessageParam<T1, T2, T3> Result() => (IMessageParam<T1, T2, T3>)ReceiveAsync(t => t is IMessageParam<T1, T2, T3>).Result;

        public IMessageParam<T1, T2, T3> Result(int timeOutMS) => (IMessageParam<T1, T2, T3>)ReceiveAsync(t => t is IMessageParam<T1, T2, T3>, timeOutMS).Result;

        public async Task<IMessageParam<T1, T2, T3>> ResultAsync() => (IMessageParam<T1, T2, T3>)await ReceiveAsync(t => t is IMessageParam<T1, T2, T3>).ConfigureAwait(false);

        public async Task<IMessageParam<T1, T2, T3>> ResultAsync(int timeOutMS) => (IMessageParam<T1, T2, T3>)await ReceiveAsync(t => t is IMessageParam<T1, T2, T3>, timeOutMS).ConfigureAwait(false);
    }
}
