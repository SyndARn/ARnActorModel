using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using Actor.Base;
using System.Threading.Tasks;
using System.Linq;

namespace TestActor
{
    /// <summary>
    /// Description résumée pour EchoServerActorTest
    /// </summary>
    [TestClass]
    public class EchoServerActorTest
    {
        [TestMethod]
        public void EchoClient100Test()
        {
            GlobalContext.MessageTracerService = new MemoryMessageTracerService();
            TestLauncherActor.Test(() =>
            {
                ActorServer.Start(ActorConfigManager.CastForTest());
                new EchoServerActor();
                for (int i = 0; i < 5; i++)
                {
                    EchoClientActor aClient = new EchoClientActor();
                    aClient.Connect("EchoServer");
                    aClient.SendMessage($"client-{i}");
                }
                Task.Delay(10000).Wait();
            },15000);
            var mess = GlobalContext.MessageTracerService.CopyAllMessages();
#pragma warning disable CA1827 // Do not use Count() or LongCount() when Any() can be used
            Assert.IsTrue(mess.Count(t => t.StartsWith("client receive", StringComparison.InvariantCulture)) >= 1);
#pragma warning restore CA1827 // Do not use Count() or LongCount() when Any() can be used
        }
    }
}
