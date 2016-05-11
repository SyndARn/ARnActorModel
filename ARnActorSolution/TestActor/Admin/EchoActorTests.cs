using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestActor;
using Actor.Util;

namespace Actor.Server.Tests
{
    [TestClass()]
    public class EchoActorTests
    {
        [TestMethod()]
        public void EchoActorTest()
        {
            TestLauncherActor.Test(() =>
            {
                var actor = new StateFullActor<string>();
                var echo = new EchoActor(actor, "Test Echo");
                Assert.AreEqual("Test Echo", actor.GetAsync().Result);
            });
        }
    }
}