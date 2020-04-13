using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using System.Threading.Tasks;

namespace TestActor
{
    internal class FutureAsyncActorTest : BaseActor
    {
        private string fData;

        public FutureAsyncActorTest()
        {
            Become(new Behavior<string>(t => fData = t));
            AddBehavior(new Behavior<IActor>(a => a.SendMessage(fData)));
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
                Future<string> future = new Future<string>();
                FutureAsyncActorTest actor = new FutureAsyncActorTest();
                actor.SendMessage("Test Data");
                Task<string> result = future.ResultAsync();
                actor.SendMessage((IActor)future);
                Assert.AreEqual("Test Data", result.Result);
            });
        }

        [TestMethod]
        public void FutureFilteredTest()
        {
            TestLauncherActor.Test(() =>
            {
                Future<string> future = new Future<string>();
                FutureAsyncActorTest actor = new FutureAsyncActorTest();
                actor.SendMessage("Test Data");
                actor.SendMessage(future);
                string result = future.Result(o => (string)o == "Test Data");
                Assert.AreEqual("Test Data", result);
            });
        }

        [TestMethod]
        public void FutureFilteredAsyncTest()
        {
            TestLauncherActor.Test(() =>
            {
                Future<string> future = new Future<string>();
                FutureAsyncActorTest actor = new FutureAsyncActorTest();
                actor.SendMessage("Test Data");
                Task<string> result = future.ResultAsync(o => (string)o == "Test Data");
                actor.SendMessage(future);
                Assert.AreEqual("Test Data", result.Result);
            });
        }
    }
}
