using System.Collections.Generic;
using Actor.Base;

namespace Actor.Util
{
    public class WfwStatus<T> : IWfwStatus<T>
    {
        public List<IWfwTransition<T>> TransitionList { get; private set; }
        public string Current { get; protected set; }
        public WfwStatus()
        {
            TransitionList = new List<IWfwTransition<T>>();
            Current = string.Empty;
        }

        public T Data { get; protected set; }
    }
}
