using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestActor;
using Actor.Base;
using System.Collections.Concurrent;
using System.Globalization;

namespace TestActor
{

    class SimpleSendReplyActor : BaseActor
    {
        public SimpleSendReplyActor()
        {
            Become(new SendReplyBehavior<string>());
        }
    }

    [TestClass()]
    public class SendReplyBehaviorTest
    {
        [TestMethod()]
        public void SendReplyTest()
        {
            TestLauncherActor.Test(() =>
            {
                var actor = new SimpleSendReplyActor();
                var future = new Future<IActor,string>();
                actor.SendMessage((IActor)future, "Test 1");
                Assert.AreEqual("Test 1", future.Result().Item2);
            });
        }
    }
}
