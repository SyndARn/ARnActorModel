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
    public class BrokerActorTests
    {
        [TestMethod()]
        public void BrokerActorTest()
        {
            TestLauncherActor.Test(
                () =>
                    {
                        EnumerableActor<string> memLogger = new EnumerableActor<string>();
                        IActor broker = new BrokerActor<string>();
                        foreach(var item in Enumerable.Range(1,10))
                        {
                            IActor worker = new WorkerActorTestString(memLogger);
                            broker.SendMessage(BrokerAction.RegisterWorker, worker);
                        }
                        foreach(var item in Enumerable.Range(1,10))
                        {
                            IActor client = new BaseActor();
                            string s = string.Format("Test range {0}", item);
                            broker.SendMessage(s);
                        }
                        Task.Delay(5000).Wait();
                        Assert.AreEqual(10, memLogger.Count);
                    });
        }
    }
}
