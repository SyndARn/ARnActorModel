using Actor.Base;
using System.Collections.Concurrent;

namespace Actor.Server
{

    public interface IActorFactory
    {
        IActor CastNewActor(string actorAddress);
    }
}
