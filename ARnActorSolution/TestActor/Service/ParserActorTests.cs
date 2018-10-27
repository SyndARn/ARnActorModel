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
            : base()
        {
            Become(new Behavior<IActor>(DoParser));
        }

        private void DoParser(IActor anActor)
        {
            List<String> aList = new List<String>
            {
                ActorTask.Stat()
            };
            var lParser = new ParserActor();
            lParser.SendMessage(aList.AsEnumerable<String>(), anActor);
        }
    }

    internal class TestReceiver : BaseActor
    {
        private readonly List<string> fList = new List<string>();

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

        private void Result(IActor aFuture)
        {
            aFuture.SendMessage(fList.AsEnumerable());
        }

        private void Receive(IEnumerable<string> list)
        {
            foreach (var item in list)
            {
                fList.Add(item);
            }
        }
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
               ConfigurationManager.AppSettings["ListenerService"] = "MemoryListenerService";
               ConfigurationManager.AppSettings["SerializeService"] = "NetDataContractSerializeService";
               ActorServer.Start("localhost", 80, new HostRelayActor());
               var parserTest = new ParserTest();
               var receiver = new TestReceiver();
               parserTest.SendMessage((IActor)receiver);
               var future = receiver.GetResult().Result();
               Assert.IsTrue(future.Any());
               Assert.IsTrue(future.Contains("Max"));
           });
        }
    }
}