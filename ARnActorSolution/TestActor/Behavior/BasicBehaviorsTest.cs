using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using System.Linq;

namespace TestActor
{

    [TestClass]
    public class BasicBehaviorsTest
    {
        [TestMethod]
        public void BehaviorsSimpleTest()
        {
            var actor = new BaseActor();
            var behaviors = new Behaviors();

            behaviors.LinkToActor(actor);
            Assert.AreEqual(actor, behaviors.LinkedActor);

            var behavior1 = new Behavior<string>();
            var behavior2 = new Behavior<int>();
            behaviors.AddBehavior(behavior1);
            behaviors.AddBehavior(behavior2);
            behaviors.AddBehavior(null);
            var allBehaviors = behaviors.AllBehaviors();
            Assert.AreEqual(2, allBehaviors.Count());
            Assert.IsTrue(allBehaviors.Contains(behavior1));
            Assert.IsTrue(allBehaviors.Contains(behavior2));

            Assert.IsTrue(behaviors.FindBehavior(behavior1));
            Assert.IsTrue(behaviors.FindBehavior(behavior2));
            var behavior3 = new Behavior<object>();
            Assert.IsFalse(behaviors.FindBehavior(behavior3));
            Assert.IsFalse(behaviors.FindBehavior(null));

            Assert.IsTrue(behavior1.LinkedTo == behaviors);
            Assert.IsTrue(behavior2.LinkedTo == behaviors);
            behaviors.RemoveBehavior(behavior2);
            Assert.IsFalse(behaviors.FindBehavior(behavior2));
            Assert.IsTrue(behaviors.FindBehavior(behavior1));
            Assert.IsTrue(behavior1.LinkedTo == behaviors);
            Assert.IsFalse(behavior2.LinkedTo == behaviors);

            Assert.IsTrue(behavior1.LinkedActor == actor);
            Assert.IsTrue(behavior2.LinkedActor == null);

        }
    }
}
