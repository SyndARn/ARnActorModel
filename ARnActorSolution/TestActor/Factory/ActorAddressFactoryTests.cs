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
            var addressFactory = new ActorAddressFactory();
            var actorFactory = new ActorFactory();
            addressFactory.CreateActorAddress(address, actorFactory);
            var actor = addressFactory.GetActor(address);
            Assert.IsNotNull(actor);
        }

    }
}