using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{
    public class bhvObservable : Behaviors
    {
    }

    public class bhvObservable<T> : bhvBehavior<T>
    {
        // ObserverList => who observe
        // ObservableList => who is observed
        // register/unregister Observer
        // register/unregister Observable
        // Event from Observable
        // Push to Observer
    }

    public class ObservableMessage<T>
    {

    }
}
