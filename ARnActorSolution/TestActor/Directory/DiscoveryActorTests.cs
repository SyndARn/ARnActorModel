using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Actor.Base;

namespace TestActor
{
    [TestClass()]
    public class DiscoveryActorTests
    {
        [TestMethod()]
        public void DiscoveryActorTest()
        {
            TestLauncherActor.Test(DoDiscoveryActorTest);
        }

        private void DoDiscoveryActorTest()
        {
            ConfigurationManager.AppSettings["ListenerService"] = "MemoryListenerService";
            ConfigurationManager.AppSettings["SerializeService"] = "NetDataContractSerializeService";
            ActorServer.Start("localhost", 80, new HostRelayActor());
            var future = new Future<Dictionary<string, string>>();
            var disco = new DiscoveryActor("localhost",future);
            var result = future.Result(60000);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count >= 3,$"result is {result.Count}");
            Assert.IsTrue(result.Keys.Contains("KnownShards"));
        }
    }
}