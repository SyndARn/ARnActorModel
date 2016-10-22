using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using System.Linq;

namespace TestActor
{
    [TestClass]
    public class NullBehaviorTest
    {
        [TestMethod]
        public void TestNullBehavior()
        {
            TestLauncherActor.Test(() =>
                {
                    var actor = new BaseActor(new NullBehavior());
                    actor.SendMessage("delta");
                });
        }

        [TestMethod]
        public void TestNullBehaviorS()
        {
            var nullBhvs = new NullBehaviors();
            Assert.IsTrue(nullBhvs.AllBehaviors().Count() == 1);
            Assert.IsTrue(nullBhvs.AllBehaviors().First() is NullBehavior);
        }
    }
}
