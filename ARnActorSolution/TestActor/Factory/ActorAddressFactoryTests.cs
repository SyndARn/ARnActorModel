using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Server.Tests
{
    [TestClass()]
    public class ActorAddressFactoryTests
    {
        [TestMethod()]
        public void HashAddressTest()
        {
            var address = @"arnactorserver/test";
            var actorFactory = new ActorFactory();
            var addressFactory = new ActorAddressFactory(actorFactory);
            addressFactory.CreateActorAddress(address);
            var actor = addressFactory.GetActor(address);
            Assert.IsNotNull(actor);
        }

    }
}