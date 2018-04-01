using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{
    public interface IStateFullActor<T>
    {
        void SetState(T aT);
        IFuture<T> GetState();
        Task<T> GetStateAsync();
        Task<T> GetStateAsync(int timeOutMS);
    }
}
