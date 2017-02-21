using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using TestActor;

namespace Actor.Service.Tests
{
    [TestClass()]
    public class LoggerActorTests
    {
        [TestMethod()]
        public void LoggerActorTest()
        {
            TestLauncherActor.Test(() =>
            {
                var logger = new LoggerActor("testLog");
                logger.MessageTracerService = new MemoryMessageTracerService();
                logger.SendMessage("Test Message");
                var msgList = logger.MessageTracerService.CopyAllMessages();
                Assert.IsTrue(msgList.Contains("Test Message"));
                Task.Delay(1000).Wait();
                msgList = logger.MessageTracerService.CopyAllMessages();
                Assert.IsTrue(msgList.Any(s => s.Contains("HeartBeatAction")));
            });
        }
    }
}