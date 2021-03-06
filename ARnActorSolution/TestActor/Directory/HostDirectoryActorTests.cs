﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;
using Actor.Base;
using Actor.Server;
using TestActor;

namespace Actor.Server.Tests
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
#pragma warning disable CA1827 // Do not use Count() or LongCount() when Any() can be used
                Assert.IsTrue(stat2.Count(t => t == actor.Tag.Key())== 0); // don't touch
#pragma warning restore CA1827 // Do not use Count() or LongCount() when Any() can be used
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Supprimer les objets avant la mise hors de portée")]
        [TestMethod()]
        public void RegisterUnregisterTest()
        {
            TestLauncherActor.Test(() =>
            {
                ActorServer.Start(ActorConfigManager.CastForTest());
                var actor = new StateFullActor<string>();
                HostDirectoryActor.Register(actor);
                SerialObject so = new SerialObject(new MessageParam<StateAction,string>(StateAction.Set,"Test"), actor.Tag);
                HostDirectoryActor.GetInstance().SendMessage(so);
                Task.Delay(5000).Wait();
                var result = actor.GetStateAsync(5000).Result;
                Assert.AreEqual(result, "Test");

                HostDirectoryActor.Unregister(actor);
                SerialObject so2 = new SerialObject(new MessageParam<StateAction,string>(StateAction.Set, "Test2"), actor.Tag);
                HostDirectoryActor.GetInstance().SendMessage(so2);
                Task.Delay(5000).Wait();
                var result2 = actor.GetStateAsync(5000).Result;
                Assert.AreEqual("Test",result2,string.Format(CultureInfo.InvariantCulture,"Expected {0} Found {1}","Test",result2));
            });
        }
    }
}