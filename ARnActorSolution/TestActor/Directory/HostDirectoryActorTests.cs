using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using System;
using Actor.Util;
using System.Configuration;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;
using Actor.Base;

namespace TestActor
{
    [TestClass()]
    public class HostDirectoryActorTests
    {
        [TestMethod()]
        public void GetStatTest()
        {
            var stat = HostDirectoryActor.GetInstance().GetStat();
            Assert.IsNotNull(stat);
            Assert.IsTrue(stat.Contains("Host entries"));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Supprimer les objets avant la mise hors de portée")]
        [TestMethod()]
        public void RegisterUnregisterTestV2()
        {
            TestLauncherActor.Test(() =>
            {
                ActorServer.Start(ActorConfigManager.CastForTest());
                var actor = new StateFullActor<string>();

                HostDirectoryActor.Register(actor);
                Task.Delay(5000).Wait();
                var stat = HostDirectoryActor.GetInstance().GetEntries();
                Assert.IsTrue(stat.Count(t => t == actor.Tag.Key()) == 1);

                HostDirectoryActor.Unregister(actor);
                Task.Delay(5000).Wait();
                var stat2 = HostDirectoryActor.GetInstance().GetEntries();
                Assert.IsTrue(stat2.Count(t => t == actor.Tag.Key()) == 0);
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Supprimer les objets avant la mise hors de portée")]
        [TestMethod()]
        //[Ignore]
        public void RegisterUnregisterTest()
        {
            TestLauncherActor.Test(() =>
            {
                ActorServer.Start(ActorConfigManager.CastForTest());
                var actor = new StateFullActor<string>();
                HostDirectoryActor.Register(actor);
                SerialObject so = new SerialObject(new MessageParam<StateAction,string>(StateAction.Set,"Test"), actor.Tag);
                HostDirectoryActor.GetInstance().SendMessage(so);
                var result = actor.GetStateAsync(10000).Result;
                Assert.AreEqual("Test",result);

                HostDirectoryActor.Unregister(actor);
                SerialObject so2 = new SerialObject(new MessageParam<StateAction,string>(StateAction.Set, "Test2"), actor.Tag);
                HostDirectoryActor.GetInstance().SendMessage(so2);
                var result2 = actor.GetStateAsync(10000).Result;
                Assert.AreEqual("Test",result2,string.Format(CultureInfo.InvariantCulture,"Expected {0} Found {1}","Test",result2));
            });
        }
    }
}