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

        [TestMethod()]
        [Ignore]
        public void ParserActorTest()
        {
            var testLauncher = new TestLauncherActor();
            testLauncher.SendAction(() =>
           {
               var parserTest = new ParserTest();
               var receiver = new TestReceiver();
               parserTest.SendMessage((IActor)receiver);
               var future = receiver.GetResult().ResultAsync().Result;
               Assert.IsTrue(future.Any());
               Assert.IsTrue(future.Contains("Max"));
               testLauncher.Finish();
           });
            Assert.IsTrue(testLauncher.Wait());
        }
    }
}