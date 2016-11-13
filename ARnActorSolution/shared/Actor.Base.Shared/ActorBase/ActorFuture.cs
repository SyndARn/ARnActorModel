using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public T Result() => (T)Receive(t => t is T).Result ;

        public T Result(int timeOutMS) => (T)Receive(t => t is T, timeOutMS).Result ;

        public async Task<T> ResultAsync()
        {
            return (T)await Receive(t => t is T);
        }

        public async Task<T> ResultAsync(int timeOutMS)
        {
            return (T)await Receive(t => t is T, timeOutMS) ;
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
