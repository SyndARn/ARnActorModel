using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "bhv")]
    public class bhvObservable : Behaviors
    {
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "bhv")]
    public class bhvObservable<T> : Behavior<T>
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
