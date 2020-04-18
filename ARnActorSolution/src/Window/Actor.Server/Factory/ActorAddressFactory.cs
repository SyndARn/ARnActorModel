using Actor.Base;
using System.Collections.Concurrent;

namespace Actor.Server
{
    public class ActorAddressFactory
    {
        public static long HashAddress(string anAddress)
        {
            CheckArg.Address(anAddress);
            var hash = anAddress.GetHashCode();
            return hash;
        }

        private readonly IActorFactory _actorFactory;
        private readonly ConcurrentDictionary<string, IActor> _dico = new ConcurrentDictionary<string, IActor>() ;

        public ActorAddressFactory(IActorFactory actorFactory) => _actorFactory = actorFactory;

        public IActor GetActor(string actorAddress) => _dico[actorAddress];

        public void CreateActorAddress(string actorAddress) => _dico[actorAddress] = _actorFactory.CastNewActor(actorAddress);
    }
}
