using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using TestActor;

namespace Actor.Server.Tests
{
    [TestClass()]
    public class HeartBeatActorTests
    {

        class TestHeartBeatActor : BaseActor
        {
            public bool fHeartBeatReceive = false;
            public TestHeartBeatActor()
            {
                Become(new Behavior<HeartBeatActor, HeartBeatAction>
                    (
                    (a,h) =>
                    {
                        fHeartBeatReceive = true;
                    }
                    ));
            }
        }

        [TestMethod()]
        public void HeartBeatActorTest()
        {
            var heartBeat = new HeartBeatActor(5000);
            TestHeartBeatActor actor = new TestHeartBeatActor();
            TestLauncherActor.Test(
           () =>
           {
               heartBeat.SendMessage((IActor)actor);
               Task.Delay(1000).Wait();
               Assert.IsTrue(actor.fHeartBeatReceive);
               actor.fHeartBeatReceive = false;
               heartBeat.SendMessage(actor);
               Task.Delay(1000).Wait();
               Assert.IsFalse(actor.fHeartBeatReceive);
               Task.Delay(5000).Wait();
               Assert.IsTrue(actor.fHeartBeatReceive);
           }
           , 20000);
            
        }
    }
}