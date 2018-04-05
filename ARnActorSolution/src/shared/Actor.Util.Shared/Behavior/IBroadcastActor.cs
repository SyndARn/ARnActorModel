using System.Collections.Generic;
using Actor.Base;

namespace Actor.Util
{
    public interface IBroadCastActor<T>
    {
        void BroadCast(T at, IEnumerable<IActor> list);
    }
}
