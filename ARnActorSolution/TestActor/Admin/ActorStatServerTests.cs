using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestActor;
using Actor.Base;

namespace Actor.Server.Tests
{
    [TestClass()]
    [Ignore]
    public class ActorStatServerTests
    {
        [TestMethod()]
        public void ActorStatServerTest()
        {
            TestLauncherActor.Test(() =>
            {
                var stat = new ActorStatServer();
                var future = new Future<string>();
                stat.SendMessage(future);
                var result = future.Result();
                Assert.IsTrue(result.Contains("Task"));
            });
        }
    }
}