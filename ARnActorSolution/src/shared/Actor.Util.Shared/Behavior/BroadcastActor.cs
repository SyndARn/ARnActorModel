using System.Collections.Generic;
using Actor.Base;

namespace Actor.Util
{
    public class BroadCastActor<T> : BaseActor, IBroadCastActor<T>
    {
        public BroadCastActor()
            : base()
        {
            Become(new BroadCastBehavior<T>());
        }

        public void BroadCast(T at, IEnumerable<IActor> list)
        {
            this.SendMessage(at, list);
        }

    }
}