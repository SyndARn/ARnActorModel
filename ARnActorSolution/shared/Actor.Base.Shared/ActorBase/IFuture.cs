using System;
using System.Threading.Tasks;

namespace Actor.Base
{

    public interface IFuture : IActor
    {
        Task<object> GetResultAsync();
    }

    public interface IFuture<T> : IFuture
    {
        T Result();
        T Result(int timeOutMS);
        Task<T> ResultAsync();
        Task<T> ResultAsync(int timeOutMS);
    }

    public interface IFuture<T1, T2> : IFuture
    {
        Tuple<T1, T2> Result();
        Tuple<T1, T2> Result(int timeOutMS);
        Task<Tuple<T1, T2>> ResultAsync();
        Task<Tuple<T1, T2>> ResultAsync(int timeOutMS);
    }
}