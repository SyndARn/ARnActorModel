using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Util;
using System.Configuration;

namespace TestActor
{

    internal class TestHostRelayActor : BaseActor
    {
        private List<string> Data = new List<string>();

        public TestHostRelayActor()
        {
            Become(new Behavior<Dictionary<string, string>>(
                d =>
                {
                    Data.Clear();
                    Data.AddRange(d.Keys);
                    Become(new Behavior<IFuture<IEnumerable<string>>>(
                        f => f.SendMessage(Data.AsEnumerable<string>())
                        ));
                }
                )) ;
        }
    }

    [TestClass()]
    public class HostRelayActorTests
    {
        [TestMethod()]
        public void HostRelayDiscoTest()
        {
            TestLauncherActor.Test(() => DoDiscoTest(),30000);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Supprimer les objets avant la mise hors de portée")]
        private void DoDiscoTest()
        {
            ActorServer.Start(ConfigManager.CastForTest());
            IActor actor1 = new TestHostRelayActor();

            DiscoCommand disco = new DiscoCommand(actor1);
            RemoteSenderActor remote = new RemoteSenderActor(actor1.Tag);
            remote.SendMessage(disco);

            IFuture<IEnumerable<string>> future = new Future<IEnumerable<string>>();
            actor1.SendMessage(future);
            var result1 = future.Result();
            Assert.IsTrue(result1.Count() == 3);
        }

    }
}