using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using Actor.Base;

namespace TestActor
{
    [TestClass]
    public class ActorQueueTest
    {
        actTestLauncher fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new actTestLauncher();
        }



        [TestMethod]
        public void TestActorQueue()
        {
            fLauncher.SendAction(() =>
                {
                    var actorqueue = new actQueue<string>();
                    actorqueue.Queue("a");
                    actorqueue.Queue("b");
                    actorqueue.Queue("c");
                    var a = actorqueue.TryDequeue();
                    var b = actorqueue.TryDequeue();
                    var c = actorqueue.TryDequeue();
                    Assert.IsTrue(a.Result.Item1);
                    Assert.IsTrue(b.Result.Item1);
                    Assert.IsTrue(c.Result.Item1);
                    string s = a.Result.Item2 + b.Result.Item2 + c.Result.Item2;
                    Assert.AreEqual(3, s.Length);
                    Assert.IsTrue(s.Contains("a"));
                    Assert.IsTrue(s.Contains("b"));
                    Assert.IsTrue(s.Contains("c"));
                    fLauncher.Finish();
                });
            fLauncher.Wait();
        }
    }
}
