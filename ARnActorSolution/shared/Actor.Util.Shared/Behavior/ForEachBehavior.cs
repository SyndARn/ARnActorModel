using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{
    /// <summary>
    /// ForEachBehavior
    ///   Apply an Action on an IEnumerable by creating an actor for each item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ForEachBehavior<T> : Behavior<IEnumerable<T>, Action<T>>
    {
        public ForEachBehavior() : base()
        {
            this.Pattern = (e,a) => { return true; };
            this.Apply = ForEach;
        }

        private void ForEach(IEnumerable<T> list, Action<T> action)
        {
            foreach (T act in list)
            {
                new BaseActor(new DoForEachbehavior<T>()).SendMessage(act, action);
            }
        }
    }

    internal class DoForEachbehavior<T> : Behavior<T, Action<T>>
    {
        public DoForEachbehavior()
        {
            this.Pattern = (t,a) => { return true; };
            this.Apply = DoEach;
        }

        private void DoEach(T aT, Action<T> action) => action(aT);
    }
}

