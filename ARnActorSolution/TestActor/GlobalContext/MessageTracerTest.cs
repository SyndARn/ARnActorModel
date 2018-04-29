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
            IMessageTracerService mts = new MemoryMessageTracerService();

            TestLauncherActor.Test(() =>
            {
                BaseActor actor1 = new BaseActor()
                {
                    MessageTracerService = mts
                };
                actor1.SendMessage("Test 1");
                actor1.SendMessage("Test 2");
                actor1.SendMessage("Test 3");
            });

            var query = mts.CopyAllMessages().ToList();
            string s = string.Empty;
            query.ForEach(item => s = s + item);
            Assert.AreEqual(3, query.Count, s);
            Assert.IsTrue(query.Contains("Test 2"));
            Assert.IsFalse(query.Contains("Test 4"));
            Assert.AreEqual("Test 1", query.First());
            Assert.AreEqual("Test 3", query.Last());
        }
    }
}
