using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Actor.Base;
using System.Configuration;
using TestActor;

namespace Actor.Server.Tests
{
    internal class TestHostRelayActor : BaseActor
    {
        private readonly List<string> _data = new List<string>();

        public TestHostRelayActor()
        {
            Become(new Behavior<Dictionary<string, string>>(
                d =>
                {
                    _data.Clear();
                    _data.AddRange(d.Keys);
                    Become(new Behavior<IFuture<IEnumerable<string>>>(
                        f => f.SendMessage(_data.AsEnumerable())
                        ));
                }
                ));
        }
    }

    [TestClass()]
    public class HostRelayActorTests
    {
        [TestMethod()]
        public void HostRelayDiscoTest()
        {
            TestLauncherActor.Test(() => DoDiscoTest());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Supprimer les objets avant la mise hors de portée")]
        private void DoDiscoTest()
        {
            ConfigurationManager.AppSettings["ListenerService"] = "MemoryListenerService";
            ConfigurationManager.AppSettings["SerializeService"] = "NetDataContractSerializeService";
            ActorConfigManager config = ActorConfigManager.CastForTest();
            ActorServer.Start(config);
            IActor actor1 = new TestHostRelayActor();

            DiscoCommand disco = new DiscoCommand(actor1);
            RemoteSenderActor remote = new RemoteSenderActor(actor1.Tag);
            remote.SendMessage(disco);

            IFuture<IEnumerable<string>> future = new Future<IEnumerable<string>>();
            actor1.SendMessage(future);
            var result1 = future.Result();
            Assert.IsTrue(result1.Count() >= 3, $"found {result1.Count()}");
        }
    }
}