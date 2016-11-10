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
using System.Globalization;

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
                        Enumerable.Range(1, 10).All((t) =>
                         {
                             IActor worker = new WorkerActorTestString(memLogger);
                             broker.SendMessage(BrokerAction.RegisterWorker, worker);
                             return true;
                         });
                        Enumerable.Range(1, 10).All(t =>
                         {
                             IActor client = new BaseActor();
                             string s = string.Format(CultureInfo.InvariantCulture, "Test range {0}", t);
                             broker.SendMessage(s);
                             return true;
                         });
                        Task.Delay(5000).Wait();
                        Assert.AreEqual(10, memLogger.Count);
                    });
        }
    }
}
