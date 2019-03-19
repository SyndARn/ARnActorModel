using System.Collections.Generic;
using Actor.Base;

namespace Actor.Util
{
    public class BroadCastBehavior<T> : Behavior<T, IEnumerable<IActor>>
    {
        public BroadCastBehavior()
        {
            this.Pattern = (t, en) => true;
            this.Apply = Behavior;
        }

        private void Behavior(T at, IEnumerable<IActor> actors)
        {
            foreach (IActor actor in actors)
            {
                actor.SendMessage(at);
            }
        }
    }
}
