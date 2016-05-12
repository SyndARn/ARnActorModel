using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;

namespace TestActor.ActorBase
{

    class ActorTest : BaseActor
    {
        private string fData;
        public ActorTest()
        {
            Become(new Behavior<string>(t =>
            {
                fData = t;
            }));
            AddBehavior(new Behavior<IActor>(a =>
            {
                a.SendMessage(fData);
            }));
        }
    }

    [TestClass]
    public class FutureTest
    {        

        [TestMethod]
        public void FutureAsyncTest()
        {
            TestLauncherActor.Test(() =>
            {
                var future = new Future<string>();
                var actor = new ActorTest();
                actor.SendMessage("Test Data");
                Assert.AreEqual("Test Data", future.ResultAsync().Result);
            });
        }
    }
}
