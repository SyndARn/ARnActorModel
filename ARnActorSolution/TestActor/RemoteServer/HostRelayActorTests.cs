using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Actor.Base;
using Actor.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestActor
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
            //ActorServer.Start("localhost", 80, new HostRelayActor());
            ActorConfigManager config = ActorConfigManager.CastForTest();
            ActorServer.Start(config);
            IActor actor1 = new TestHostRelayActor();

            var disco = new DiscoCommand(actor1);
            var remote = new RemoteSenderActor(actor1.Tag);
            remote.SendMessage(disco);

            IFuture<IEnumerable<string>> future = new Future<IEnumerable<string>>();
            actor1.SendMessage(future);
            IEnumerable<string> result1 = future.Result();
            Assert.IsTrue(result1.Skip(3 - 1).Any(), $"found {result1.Count().ToString()}");
        }
    }
}