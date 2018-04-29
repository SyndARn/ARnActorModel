using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using TestActor;
using System.IO;

namespace Actor.Service.Tests
{
    [TestClass()]
    public class LoggerActorTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod()]
        public void LoggerActorTest()
        {
            const string logFilename = "testLog";
            TestLauncherActor.Test(() =>
            {
                var logger = new LoggerActor(logFilename);
                logger.MessageTracerService = new MemoryMessageTracerService();
                logger.SendMessage("Test Message");
                var msgList = logger.MessageTracerService.CopyAllMessages();
                Assert.IsTrue(msgList.Contains("Test Message"));
                Task.Delay(1000).Wait();
                msgList = logger.MessageTracerService.CopyAllMessages();
                Assert.IsTrue(msgList.Any(s => s.Contains("HeartBeatAction")));
                Task.Delay(1000).Wait();
            });
            var file = Path.Combine(Environment.CurrentDirectory, logFilename);
            List<string> strings = new List<string>();
            using (var stream = new StreamReader(file))
            {
                while (!stream.EndOfStream)
                {
                    strings.Add(stream.ReadLine());
                }
            }
            Assert.IsTrue(strings.Any(t => t.Contains("Test Message")));
        }
    }
}