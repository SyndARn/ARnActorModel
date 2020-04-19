using System.Collections.Generic;
using Actor.Base;

namespace Actor.Server
{
    public class ServerCommandService : Behaviors, IServerCommandService
    {
        public void RegisterCommand(IActorServerCommand command)
        {
            AddBehavior(command);
        }

        public void UnregisterCommand(IActorServerCommand command)
        {
            RemoveBehavior(command);
        }
    }
}
