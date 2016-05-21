using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace TestActor
{
    [TestClass()]
    public class ProcessActorTests
    {
        private IActor pong;
        private IActor ping;
        [TestMethod()]
        public void ProcessActorTest()
        {
            TestLauncherActor.Test(() =>
            {
                var futurePing = new Future<string>();
                var futurePong = new Future<string>();
                ping = new ProcessActor<string>(msg =>
                {
                    futurePing.SendMessage(msg);
                });
                pong = new ProcessActor<string>(msg =>
                {
                    futurePong.SendMessage(msg);
                    ping.SendMessage("ping");
                });
                pong.SendMessage("pong");
                Assert.AreEqual("ping", futurePing.Result());
                Assert.AreEqual("pong", futurePong.Result());
            });
        }
    }
}