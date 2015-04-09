using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{

    public class actBroadCast<T> : actActor
    {
        public actBroadCast()
            : base()
        {
            Become(new bhvBroadCast<T>()) ;
        }

        public void BroadCast(T at, IEnumerable<IActor> list)
        {
            SendMessage(Tuple.Create(at, list));
        }

    }

    public class bhvBroadCast<T> : bhvBehavior<Tuple<T, IEnumerable<IActor>>>
    {

        public bhvBroadCast()
        {
            this.Apply = Behavior ;
            this.Pattern = t => true ;
        }

        private void Behavior(Tuple<T, IEnumerable<IActor>> msg)
        {
            foreach(IActor t in msg.Item2)
            
            {
                T brd = msg.Item1 ;
                t.SendMessage(brd);
            }             
        }
    }
}
