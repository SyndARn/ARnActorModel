using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Collections.Concurrent;

namespace Actor.Server.Factory
{

    public class ActorFactory : IActorFactory
    {
        public IActor CastNewActor(string actorAddress)
        {
            var actor = new BaseActor(); // and force tag
            return actor;
        }
    }

    public interface IActorFactory
    {
        IActor CastNewActor(string actorAddress);
    }

    public class ActorAddressFactory
    {
        public static long HashAddress(string anAddress)
        {
            var hash = anAddress.GetHashCode();
            return hash;
        }

        private ConcurrentDictionary<string, IActor> fDico = new ConcurrentDictionary<string, IActor>() ;

        public ActorAddressFactory()
        {
        }

        public IActor GetActor(string actorAddress)
        {
            return fDico[actorAddress];
        }

        public void CreateActorAddress(string actorAddress, IActorFactory factory)
        {
            fDico[actorAddress] = factory.CastNewActor(actorAddress);
        }
    }

}
