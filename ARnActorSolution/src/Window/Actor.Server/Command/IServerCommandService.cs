using Actor.Base;

namespace Actor.Server
{
    public interface IServerCommandService : IBehaviors
    {
        void RegisterCommand(IActorServerCommand command);
        void UnregisterCommand(IActorServerCommand command);
    }
}
