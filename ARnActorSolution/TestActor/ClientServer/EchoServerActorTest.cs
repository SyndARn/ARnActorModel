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
                ActorServer.Start(ConfigManager.CastForTest());
                new EchoServerActor();
                for (int i = 0; i < 5; i++)
                {
                    EchoClientActor aClient = new EchoClientActor();
                    aClient.Connect("EchoServer");
                    aClient.SendMessage("client-" + i.ToString());
                }
                Task.Delay(10000).Wait();
            },15000);
            var mess = GlobalContext.MessageTracerService.CopyAllMessages();
            Assert.IsTrue(mess.Count(t => t.StartsWith("client receive")) >= 1);
        }
    }
}
