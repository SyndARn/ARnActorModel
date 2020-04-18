using Actor.Base;
using System.Collections.Concurrent;

namespace Actor.Server
{
    public class ActorFactory : IActorFactory
    {
        public IActor CastNewActor(string actorAddress)
        {
            var actor = new BaseActor(); // and force tag
            return actor;
        }
    }
}
