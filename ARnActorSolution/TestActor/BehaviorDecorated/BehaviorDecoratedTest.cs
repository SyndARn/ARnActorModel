using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Util;
using Actor.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestActor
{

    internal class TestActor : BehaviorDecoratedActor
    {
        private string fAnswer;
        private IFuture<string> future;

        [Behavior]
        public void DecoratedMessage(string s)
        {
            fAnswer = s;
            future.SendMessage(s);
        }

        public IFuture<string> GetAnswer()
        {
            future = new Future<string>();
            return future;
        }
    }

    [TestClass()]
    public class DecoratedActorTest
    {
        [TestMethod()]
        public void TestADecoratedActor()
        {
            TestLauncherActor.Test(() =>
            {
                var actor = new TestActor();
                var future = actor.GetAnswer();
                actor.SendMessage("Test Decorated");
                Assert.AreEqual("Test Decorated", future.Result());
            });
        }
    }
}