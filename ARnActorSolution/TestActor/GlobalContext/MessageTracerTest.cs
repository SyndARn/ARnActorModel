using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;

namespace TestActor
{
    [TestClass]
    public class MessageTracerTest
    {
        [TestMethod]
        public void TestMessageTracer()
        {
            GlobalContext.MessageTracerService = new MemoryMessageTracerService();

            TestLauncherActor.Test(() =>
            {
                IActor actor1 = new BaseActor();
                actor1.SendMessage("Test 1");
                actor1.SendMessage("Test 2");
                actor1.SendMessage("Test 3");
            });

            var query = GlobalContext.MessageTracerService.GetMessages().ToList();
            Assert.AreEqual(5, query.Count());
            Assert.IsTrue(query.Contains("Test 2"));
            Assert.IsFalse(query.Contains("Test 4"));
            Assert.AreEqual("System.Action", query.First());
            Assert.AreEqual("True", query.Last());
            GlobalContext.MessageTracerService = null;
        }
    }
}
