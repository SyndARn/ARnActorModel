using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using Actor.Base;

namespace TestActor
{
    [TestClass]
    public class ActorQueueTest
    {
        TestLauncherActor fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new TestLauncherActor();
        }



        [TestMethod]
        public void TestActorQueue()
        {
            fLauncher.SendAction(() =>
                {
                    var actorqueue = new QueueActor<string>();
                    actorqueue.Queue("a");
                    actorqueue.Queue("b");
                    actorqueue.Queue("c");
                    var a = actorqueue.TryDequeue();
                    var b = actorqueue.TryDequeue();
                    var c = actorqueue.TryDequeue();
                    Assert.IsTrue(a.Result.Result);
                    Assert.IsTrue(b.Result.Result);
                    Assert.IsTrue(c.Result.Result);
                    string s = a.Result.Data + b.Result.Data + c.Result.Data;
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
