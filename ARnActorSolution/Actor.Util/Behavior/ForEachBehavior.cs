using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{


    public class ForEachBehavior<T> : Behavior<Tuple<IEnumerable<T> , Action<T>>>
    {
        public ForEachBehavior() : base()
        {
            this.Pattern = t => { return true; };
            this.Apply = ForEach ;
        }

        private void ForEach(Tuple<IEnumerable<T> , Action<T>> msg)
        {
            foreach(T act in msg.Item1)
            {
                new BaseActor(new DoForEachbehavior<T>()).SendMessage(Tuple.Create(act,msg.Item2)) ;
            }
        }
    }


        internal class DoForEachbehavior<T> : Behavior<Tuple<T,Action<T>>>
        {
            public DoForEachbehavior()
            {
                this.Pattern = t => { return true ;} ;
                this.Apply = DoEach ;
            }

            private void DoEach(Tuple<T,Action<T>> msg) => msg.Item2(msg.Item1);

        }
}

