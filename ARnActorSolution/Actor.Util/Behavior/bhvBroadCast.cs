using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "act")]
    public class actBroadCast<T> : BaseActor
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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "bhv")]
    public class bhvBroadCast<T> : Behavior<Tuple<T, IEnumerable<IActor>>>
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
