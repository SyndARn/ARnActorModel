using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;

namespace TestActor
{
    /// <summary>
    /// Description résumée pour QueueFactoryTest
    /// </summary>
    [TestClass]
    public class QueueFactoryTest
    {

        [TestMethod]
        public void LockFreeCastingTest()
        {
            QueueFactory<string>.Style = QueueStyle.LockFree;
            var lockFree = QueueFactory<string>.Cast();
            Assert.IsNotNull(lockFree);
            Assert.IsTrue(lockFree is LockFreeQueue<string>);
            lockFree.Add("Test1");
            lockFree.Add("Test2");
            Assert.AreEqual(2, lockFree.Count());
            string s;
            var result = lockFree.TryTake(out s);
            Assert.IsTrue(result);
            Assert.AreEqual("Test1",s) ;
        }

        [TestMethod]
        public void LockingCastingTest()
        {
            QueueFactory<string>.Style = QueueStyle.Locking;
            var lockFree = QueueFactory<string>.Cast();
            Assert.IsNotNull(lockFree);
            Assert.IsTrue(lockFree is LockQueue<string>);
            lockFree.Add("Test1");
            lockFree.Add("Test2");
            Assert.AreEqual(2, lockFree.Count());
            string s;
            var result = lockFree.TryTake(out s);
            Assert.IsTrue(result);
            Assert.AreEqual("Test1", s);
        }
    }
}
