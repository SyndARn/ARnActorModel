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
        public void BehaviorAndInterfaceTest()
        {
            IBehavior bhv = new Behavior<BaseActor>(
                a => { } );

            IActor iBaseActor = new BaseActor();
            Assert.IsTrue(bhv.StandardPattern(iBaseActor));

            BaseActor baseActor = new BaseActor();
            Assert.IsTrue(bhv.StandardPattern(baseActor));

            ActionActor actionActor = new ActionActor();
            Assert.IsTrue(bhv.StandardPattern(baseActor));

            IFuture future = new Future<string>();
            Assert.IsTrue(bhv.StandardPattern(future));

            IBehavior<IActor, IActor> bhv2 = new Behavior<IActor, IActor>((a1, a2) => { });
            Assert.IsTrue(bhv2.StandardPattern(new MessageParam<IActor,IActor>(new BaseActor(), new Future<string>()))) ;
        }

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
            var allBehaviors = behaviors.AllBehaviors();
            Assert.AreEqual(2, allBehaviors.Count());
            Assert.IsTrue(allBehaviors.Contains(behavior1));
            Assert.IsTrue(allBehaviors.Contains(behavior2));

            Assert.IsTrue(behaviors.FindBehavior(behavior1));
            Assert.IsTrue(behaviors.FindBehavior(behavior2));
            var behavior3 = new Behavior<object>();
            Assert.IsFalse(behaviors.FindBehavior(behavior3));

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

        [TestMethod]
        public void BehaviorsTest()
        {
            var behaviors = new Behaviors();
            Assert.IsFalse(behaviors.AllBehaviors().Any());

            var bhv1 = new Behavior(a => a != null, a => { });
            behaviors = new Behaviors(new IBehavior[] { bhv1 });
            Assert.IsTrue(behaviors.AllBehaviors().Count() == 1);
            Assert.IsTrue(behaviors.FindBehavior(bhv1));
            Assert.IsTrue(bhv1.LinkedTo == behaviors);

            var bhv2 = new Behavior<string>();
            behaviors.AddBehavior(bhv2);
            Assert.IsTrue(behaviors.AllBehaviors().Count() == 2);
            Assert.IsTrue(behaviors.FindBehavior(bhv2));

            var actor = new BaseActor(behaviors);
            Assert.IsTrue(behaviors.LinkedActor == actor);
        }

        [TestMethod]
        public void BehaviorsFunctionalTest()
        {
            IActor actor = new BaseActor();
            IBehaviors behaviors = new Behaviors();

            behaviors.LinkToActor(actor);
            Assert.AreEqual(actor, behaviors.LinkedActor);

            behaviors
                .AddBehavior(new Behavior<string>())
                .AddBehavior(new Behavior<int>());

            Assert.AreEqual(2, behaviors.AllBehaviors().Count());
            Assert.IsTrue(behaviors.AllBehaviors().Any(b => b is IBehavior<string>));
            Assert.IsTrue(behaviors.AllBehaviors().Any(b => b is IBehavior<int>));

            foreach(var bhv in behaviors.AllBehaviors())
            {
                Assert.IsTrue(bhv.LinkedTo == behaviors) ;
            }

            IBehavior bhvString = behaviors.AllBehaviors().First(b => b is IBehavior<string>);
            behaviors.RemoveBehavior(bhvString);
            Assert.IsFalse(behaviors.FindBehavior(bhvString));
            Assert.IsTrue(behaviors.AllBehaviors().Count() == 1);
            Assert.IsFalse(bhvString.LinkedTo == behaviors);

            Assert.IsTrue(bhvString.LinkedActor == null);

        }
    }
}
