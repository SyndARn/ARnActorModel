using System.Collections.Generic;
using Actor.Base;

namespace Actor.Util
{
    public class WfwTransition<T> : IWfwTransition<T>
    {
        public IWfwStatus<T> Destination { get; set; }

        public Behavior<IWfwStatus<T>> Action
        {
            get;
            set;
        }
    }
}
