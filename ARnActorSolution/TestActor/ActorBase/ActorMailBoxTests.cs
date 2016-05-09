using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestActor
{
    [TestClass()]
    public class ActorMailBoxTests
    {

        [TestMethod()]
        public void AddMissTest()
        {
            var mailBox = new ActorMailBox<string>();
            mailBox.AddMessage("Test 1");
            mailBox.AddMiss("Miss 1");
            var m1 = mailBox.GetMessage();
            var mnull = mailBox.GetMessage();
            Assert.AreEqual(m1, "Test 1");
            Assert.IsTrue(string.IsNullOrWhiteSpace(mnull));
            var totalMissed = mailBox.RefreshFromMissed();
            Assert.AreEqual(1, totalMissed);
            var missed = mailBox.GetMessage();
            Assert.AreEqual("Miss 1", missed);
        }

        [TestMethod()]
        public void GetMessageTest()
        {
            var mailBox = new ActorMailBox<string>();
            mailBox.AddMessage("Test 1");
            mailBox.AddMessage("Test 2");
            mailBox.AddMessage("Test 3");
            var m1 = mailBox.GetMessage();
            var m2 = mailBox.GetMessage();
            var m3 = mailBox.GetMessage();
            var mnull = mailBox.GetMessage();
            Assert.AreEqual(m1, "Test 1");
            Assert.AreEqual(m2, "Test 2");
            Assert.AreEqual(m3, "Test 3");
            Assert.IsTrue(string.IsNullOrWhiteSpace(mnull));
        }
    }
}