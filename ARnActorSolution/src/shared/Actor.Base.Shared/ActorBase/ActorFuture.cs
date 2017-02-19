﻿using System;
using System.Threading.Tasks;

namespace Actor.Base
{

    public class Future<T> : BaseActor, IFuture<T>
    {
        public Future()
        {
        }

        public async Task<object> GetResultAsync()
        {
            return await ResultAsync();
        }

        public T Result()
        {
            return (T)Receive(t => t is T).Result;
        }

        public T Result(int timeOutMS)
        {
            return (T)Receive(t => t is T, timeOutMS).Result;
        }

        public async Task<T> ResultAsync()
        {
            return (T)await Receive(t => t is T);
        }

        public async Task<T> ResultAsync(int timeOutMS)
        {
            return (T)await Receive(t => t is T, timeOutMS) ;
        }

        public T Result(Func<object, bool> aPattern)
        {
            return (T)Receive(aPattern).Result;
        }

        public async Task<T> ResultAsync(Func<object, bool> aPattern)
        {
            return (T)await Receive(aPattern);
        }

        public async Task<T> ResultAsync(Func<object, bool> aPattern, int timeOutMS)
        {
            return (T)await Receive(aPattern, timeOutMS);
        }

    }

    public class Future<T1,T2> : BaseActor, IFuture<T1, T2>
    {
        public Future()
        {
        }

        public async Task<object> GetResultAsync()
        {
            return await ResultAsync();
        }

        public IMessageParam<T1,T2> Result() => (IMessageParam<T1, T2>)Receive(t => t is IMessageParam<T1, T2>).Result;

        public IMessageParam<T1, T2> Result(int timeOutMS) => (IMessageParam<T1, T2>)Receive(t => t is IMessageParam<T1, T2>, timeOutMS).Result;

        public async Task<IMessageParam<T1, T2>> ResultAsync()
        {
            return (IMessageParam < T1, T2 >) await Receive(t => t is IMessageParam<T1, T2>);
        }

        public async Task<IMessageParam<T1, T2>> ResultAsync(int timeOutMS)
        {
            return (IMessageParam<T1, T2>)await Receive(t => t is IMessageParam<T1, T2>,timeOutMS);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public class Future<T1, T2, T3> : BaseActor, IFuture<T1, T2, T3>
    {
        public Future()
        {
        }

        public async Task<object> GetResultAsync()
        {
            return await ResultAsync();
        }

        public IMessageParam<T1, T2, T3> Result() => (IMessageParam<T1, T2, T3>)Receive(t => t is IMessageParam<T1, T2, T3>).Result;

        public IMessageParam<T1, T2, T3> Result(int timeOutMS) => (IMessageParam<T1, T2, T3>)Receive(t => t is IMessageParam<T1, T2, T3>, timeOutMS).Result;

        public async Task<IMessageParam<T1, T2, T3>> ResultAsync()
        {
            return (IMessageParam<T1, T2, T3>)await Receive(t => t is IMessageParam<T1, T2, T3>);
        }

        public async Task<IMessageParam<T1, T2, T3>> ResultAsync(int timeOutMS)
        {
            return (IMessageParam<T1, T2, T3>)await Receive(t => t is IMessageParam<T1, T2, T3>, timeOutMS);
        }
    }

}
