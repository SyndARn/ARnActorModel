using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Actor.Base;

namespace Actor.Server
{
    /// <summary>
    /// <para>
    /// BrokerActor
    ///     Have an actor that upon receiving message will fan out this message to one or more of pre-registrered actor
    ///     BrokerActor are very usefull if they are registered, sending a message to the broker will forward it across the
    ///     shards.
    /// </para>
    /// <para>
    ///     a simple broker send a message to one of it workers, 
    ///     a more complex broker will take a different rout : one of free worker, a new spawned worker, etc ...
    /// </para>
    /// <para>
    ///     worker can be made simple, they just react to message
    ///     or complex : they become statefull and can alert the broker of their current status (alive, ready, busy ...)
    /// </para>
    /// 
    /// </summary>
    /// 
    public enum BrokerAction
    {
        RegisterWorker,
        UnregisterWorker,
        Hearbeat,
        Start
    }
}
