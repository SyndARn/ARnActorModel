using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestActor;
using Actor.Util;
using Actor.Base;

namespace Actor.Server.Tests
{
    [TestClass()]
    public class EchoActorTests
    {
        class EchoTest : BaseActor
        {
            private string fData;
            public EchoTest() : base()
            {
                Become(new Behavior<IActor, string>((a, s) =>
                 {
                     fData = s;
                 }));
                AddBehavior(new Behavior<IFuture<string>>(a =>
                {
                    a.SendMessage(fData);
                }));
            }
        }

        [TestMethod()]
        public void EchoActorTest()
        {
            var launcher = new TestLauncherActor();
            launcher.SendAction(() =>
            {
                var actor = new EchoTest();
                var echo = new EchoActor(actor, "Test Echo");
                IFuture<string> future = new Future<string>();
                actor.SendMessage(future);
                Assert.AreEqual("Test Echo", future.ResultAsync().Result);
                launcher.Finish();
            });
            Assert.IsTrue(launcher.Wait());
        }
    }
}