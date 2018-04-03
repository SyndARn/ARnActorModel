using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{

    public interface IBroadCastActor<T>
    {
        void BroadCast(T at, IEnumerable<IActor> list);
    }

    public class BroadCastBehavior<T> : Behavior<T, IEnumerable<IActor>>
    {

        public BroadCastBehavior()
        {
            this.Pattern = (t, en) => true;
            this.Apply = Behavior ;
        }

        private void Behavior(T at, IEnumerable<IActor> actors)
        {
            foreach(IActor actor in actors)            
            {
                actor.SendMessage(at);
            }             
        }
    }
}
