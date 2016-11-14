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


        [TestMethod()]
        public void RegisterUnregisterTestV2()
        {
            TestLauncherActor.Test(() =>
            {
                ConfigurationManager.AppSettings["ListenerService"] = "MemoryListenerService";
                ConfigurationManager.AppSettings["SerializeService"] = "NetDataContractSerializeService";
                ActorServer.Start("localhost", 80, new HostRelayActor());
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

        [TestMethod()]
        [Ignore]
        public void RegisterUnregisterTest()
        {
            TestLauncherActor.Test(() =>
            {
                ConfigurationManager.AppSettings["ListenerService"] = "MemoryListenerService";
                ConfigurationManager.AppSettings["SerializeService"] = "NetDataContractSerializeService";
                ActorServer.Start("localhost", 80, new HostRelayActor());
                var actor = new StateFullActor<string>();
                HostDirectoryActor.Register(actor);
                SerialObject so = new SerialObject(new MessageParam<StateAction,string>(StateAction.Set,"Test"), actor.Tag);
                HostDirectoryActor.GetInstance().SendMessage(so);
                var result = actor.GetAsync(10000).Result;
                Assert.AreEqual(result, "Test");

                HostDirectoryActor.Unregister(actor);
                SerialObject so2 = new SerialObject(new MessageParam<StateAction,string>(StateAction.Set, "Test2"), actor.Tag);
                HostDirectoryActor.GetInstance().SendMessage(so2);
                var result2 = actor.GetAsync(10000).Result;
                Assert.AreEqual("Test",result2,string.Format(CultureInfo.InvariantCulture,"Expected {0} Found {1}","Test",result2));
            });
        }
        
    }
}