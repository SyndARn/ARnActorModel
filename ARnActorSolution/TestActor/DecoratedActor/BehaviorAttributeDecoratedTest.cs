using Actor.Util;
using Actor.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestActor;
using System.Linq;

namespace Actor.Util.Test
{
    internal class TestActorBehaviorDecorated : BaseActor
    {
        private IFuture<string> future;

        public TestActorBehaviorDecorated() : base()
        {
            Become(new BehaviorAttributeBuilder().BuildFromAttributes(this).ToArray());
        }

        [Behavior]
        internal void DecoratedMessage(string s)
        {
            future.SendMessage(s);
        }

        [Behavior]
        internal void DecoratedMessage(IActor a, string s)
        {
            a.SendMessage(s);
        }

        [Behavior]
        internal void DecoratedMessage(IActor a, string s, int i)
        {
            a.SendMessage(s, i);
        }


        public IFuture<string> GetAnswer()
        {
            future = new Future<string>();
            return future;
        }

        public void PostAnswer(IActor a, string s)
        {
            this.SendMessage(a, s);
        }

        public void PostAnswer(IActor a, string s, int i)
        {
            this.SendMessage(a, s, i);
        }
    }

    [TestClass()]
    public class BehaviorAttributeDecoratedTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod()]
        public void TestADecoratedActor()
        {
            TestLauncherActor.Test(() =>
            {
                var actor = new TestActorBehaviorDecorated();
                var future = actor.GetAnswer();
                actor.SendMessage("Test Decorated");
                Assert.AreEqual("Test Decorated", future.Result());
            });
        }

        [TestMethod()]
        public void TestADecoratedActorWith2Args()
        {
            TestLauncherActor.Test(() =>
            {
                var actor = new TestActorBehaviorDecorated();
                var future = new Future<string>();
                actor.PostAnswer(future, "Test Decorated");
                Assert.AreEqual("Test Decorated", future.Result());
            });
        }

        [TestMethod()]
        public void TestADecoratedActorWith3Args()
        {
            TestLauncherActor.Test(() =>
            {
                var actor = new TestActorBehaviorDecorated();
                var future = new Future<string, int>();
                actor.PostAnswer(future, "Test Decorated", 1);
                var result = future.Result();
                Assert.AreEqual("Test Decorated", result.Item1);
                Assert.AreEqual(1, result.Item2);
            });
        }
    }
}
