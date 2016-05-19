using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Util;

namespace TestActor
{
    [TestClass()]
    public class actParserTests
    {

        public class ParserTest : BaseActor
        {
            public ParserTest()
                : base()
            {
                Become(new Behavior<IActor>(DoParser));
            }

            private void DoParser(IActor anActor)
            {
                List<String> aList = new List<String>();
                aList.Add(ActorTask.Stat());
                var lParser = new ParserActor();
                lParser.SendMessage(aList.AsEnumerable<String>(), anActor);
            }
        }

        public class TestReceiver : BaseActor
        {
            private List<string> fList = new List<string>();
            public TestReceiver()
            {
                Become(new Behavior<string>(Receive));
                AddBehavior(new Behavior<IActor>(Result));
            }

            public Future<IEnumerable<string>> GetResult()
            {
                var future = new Future<IEnumerable<string>>();
                return future;
            }

            private void Result(IActor aFuture)
            {
                aFuture.SendMessage(fList.AsEnumerable());
            }

            private void Receive(string s)
            {
                fList.Add(s);
            }
        }

        [TestMethod()]
        public void ParserActorTest()
        {
            TestLauncherActor.Test(() =>
           {
               var parserTest = new ParserTest();
               var receiver = new TestReceiver();
               parserTest.SendMessage(receiver);
               var future = receiver.GetResult();
               Assert.IsTrue(future.Result().Any());
               Assert.IsTrue(future.Result().Contains("Max"));
           });
        }
    }
}