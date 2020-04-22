using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using Actor.Base;
using System.Configuration;
using Actor.Server;

namespace TestActor
{
    internal class ParserTest : BaseActor
    {
        public ParserTest()
            : base() => Become(new Behavior<IActor>(DoParser));

        private void DoParser(IActor anActor)
        {
            var aList = new List<string>
            {
                ActorTask.Stat()
            };
            var lParser = new ParserActor();
            lParser.SendMessage(aList.AsEnumerable(), anActor);
        }
    }

    internal class TestReceiver : BaseActor
    {
        private readonly List<string> _list = new List<string>();

        public TestReceiver()
        {
            Become(new Behavior<IEnumerable<string>>(Receive));
            AddBehavior(new Behavior<IActor>(Result));
        }

        public Future<IEnumerable<string>> GetResult()
        {
            var future = new Future<IEnumerable<string>>();
            SendMessage((IActor)future);
            return future;
        }

        private void Result(IActor future) => future.SendMessage(_list.AsEnumerable());

        private void Receive(IEnumerable<string> list) => _list.AddRange(list);
    }

    [TestClass()]
    public class ParserActorTests
    {
        [TestMethod()]
        [Ignore]
        public void ParserActorTest()
        {
            TestLauncherActor.Test(() =>
           {
               ActorServer.Start(ActorConfigManager.CastForTest());
               var parserTest = new ParserTest();
               var receiver = new TestReceiver();
               parserTest.SendMessage((IActor)receiver);
               IEnumerable<string> future = receiver.GetResult().Result();
               Assert.IsTrue(future.Any());
               Assert.IsTrue(future.Contains("Max"));
           });
        }
    }
}