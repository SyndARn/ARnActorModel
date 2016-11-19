using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using System.Threading.Tasks;
using Actor.Base;

namespace TestActor
{

    internal class TestHeartBeatActor : BaseActor
    {
        public bool fHeartBeatReceive = false;
        public TestHeartBeatActor()
        {
            Become(new Behavior<HeartBeatActor, HeartBeatAction>
                (
                (a, h) =>
                {
                    fHeartBeatReceive = true;
                }
                ));
        }
    }


    [TestClass()]
    public class HeartBeatActorTests
    {

        [TestMethod()]
        public void HeartBeatActorTest()
        {
            var heartBeat = new HeartBeatActor(5000);
            TestHeartBeatActor actor = new TestHeartBeatActor();
            TestLauncherActor.Test(
           () =>
           {
               heartBeat.SendMessage(actor);
               Task.Delay(5100).Wait();
               Assert.IsTrue(actor.fHeartBeatReceive);
               actor.fHeartBeatReceive = false;
               Assert.IsFalse(actor.fHeartBeatReceive);
               Task.Delay(5100).Wait();
               Assert.IsTrue(actor.fHeartBeatReceive);
           });
            
        }
    }
}