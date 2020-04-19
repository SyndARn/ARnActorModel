using Actor.Base;
using Actor.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestActor
{
    [TestClass()]
    public class RingActorTests
    {
        [TestMethod()]
        public void RingActorTest()
        {
            TestLauncherActor.Test(() =>
            {
                var future = new Future<string>();
                new RingActor(100, 100, future); // 10 sec

                var result = future.Result();

                Assert.IsFalse(string.IsNullOrEmpty(result));

                Assert.IsTrue(result.Contains("End Test"));

                Assert.IsTrue(result.Contains("Elapsed"));
            });
        }
    }
}