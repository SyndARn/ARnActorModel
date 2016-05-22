using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using System;
using Actor.Util;

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
        public void RegisterUnregisterTest()
        {
            TestLauncherActor.Test(() =>
            {
                var actor = new StateFullActor<string>();
                HostDirectoryActor.Register(actor);
                SerialObject so = new SerialObject(Tuple.Create(StateAction.Set,"Test"), actor.Tag);
                HostDirectoryActor.GetInstance().SendMessage(so);
                var result = actor.GetAsync().Result;
                Assert.AreEqual(result, "Test");

                HostDirectoryActor.Unregister(actor);
                SerialObject so2 = new SerialObject(Tuple.Create(StateAction.Set, "Test2"), actor.Tag);
                HostDirectoryActor.GetInstance().SendMessage(so2);
                var result2 = actor.GetAsync(1000).Result;
                Assert.AreEqual("Test",result2);
            });
        }

    }
}