using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Actor.Base;
using TestActor;

namespace Actor.Server.Tests
{
    [TestClass()]
    public class DiscoveryActorTests
    {
        [TestMethod()]
        public void DiscoveryActorTest()
        {
            TestLauncherActor.Test(DoDiscoveryActorTest);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1806:Ne pas ignorer les résultats des méthodes", Justification = "<En attente>")]
        private void DoDiscoveryActorTest()
        {
            const string localHost = "http://localhost:80";
            var uri = new Uri(localHost);
            ConfigurationManager.AppSettings["ListenerService"] = "MemoryListenerService";
            ConfigurationManager.AppSettings["SerializeService"] = "NetDataContractSerializeService";
            ActorServer.Start(ActorConfigManager.CastForTest());
            var future = new Future<Dictionary<string, string>>();
            new DiscoveryActor(uri.AbsoluteUri, future);
            var result = future.Result(60000);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count >= 3,$"result is {result.Count}");
            Assert.IsTrue(result.Keys.Contains("KnownShards"));
        }
    }
}