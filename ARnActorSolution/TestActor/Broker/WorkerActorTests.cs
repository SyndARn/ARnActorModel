using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using TestActor;
using Actor.Service;
using Actor.Util;

namespace Actor.Server.Tests
{

    [TestClass()]
    public class WorkerActorTests
    {
        [TestMethod()]
        public void WorkerActorTest()
        {
            TestLauncherActor.Test(
                () =>
                {
                    EnumerableActor<string> memLogger = new EnumerableActor<string>();
                    IActor worker = new WorkerActorTestString(memLogger);
                    IActor dummyBroker = new BaseActor();
                    ActorTag tag = new ActorTag();
                    worker.SendMessage(dummyBroker, tag, "Worker on stage");
                    Task.Delay(1000).Wait();
                    Assert.AreEqual(1, memLogger.Count);
                    Assert.AreEqual("Worker on stage", memLogger.First());
                });
        }
    }
}