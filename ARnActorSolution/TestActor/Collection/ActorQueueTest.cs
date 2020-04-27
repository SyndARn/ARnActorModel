using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestActor;

namespace Actor.Util.Test
{
    [TestClass]
    public class ActorQueueTest
    {
        [TestMethod]
        public void TestActorQueue()
        {
            TestLauncherActor.Test(() =>
                {
                    var actorqueue = new QueueActor<string>();
                    actorqueue.Queue("a");
                    actorqueue.Queue("b");
                    actorqueue.Queue("c");
                    var a = actorqueue.TryDequeueAsync();
                    var b = actorqueue.TryDequeueAsync();
                    var c = actorqueue.TryDequeueAsync();
                    Assert.IsTrue(a.Result.Result);
                    Assert.IsTrue(b.Result.Result);
                    Assert.IsTrue(c.Result.Result);
                    string s = a.Result.Data + b.Result.Data + c.Result.Data;
                    Assert.AreEqual(3, s.Length);
                    Assert.IsTrue(s.Contains("a"));
                    Assert.IsTrue(s.Contains("b"));
                    Assert.IsTrue(s.Contains("c"));
                });
        }
    }
}
