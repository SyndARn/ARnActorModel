using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestActor;
using Actor.Util;
using Actor.Base;

namespace Actor.Service.Tests
{

    class TestParserActor : BaseActor
    {
        List<string> fList = new List<string>();
        public TestParserActor()
        {
            Become(new Behavior<IActor, string>((a, s) =>
             {
                 fList.Add(s);
             }));
            AddBehavior(new Behavior<IActor>(a =>
            {
                IEnumerable<string> list = fList.AsEnumerable();
                a.SendMessage(list);
            }
            ));
        }
        public async Task<IEnumerable<string>> GetList()
        {
            var future = new Future<IEnumerable<string>>();
            SendMessage((IActor)future);
            return await future.ResultAsync();
        }
    }

    [TestClass()]
    public class ReceiveLineBehaviorTests
    {
        [TestMethod()]
        public void ReceiveLineTest()
        {
            BehaviorReceiveLine brl = new BehaviorReceiveLine();
            TestParserActor receive = new TestParserActor();
            string testLine = "A B CD E F";
            var msg = Tuple.Create((IActor)receive, testLine);
            TestLauncherActor.Test(() =>
            {
               // call behavior directly
               if (brl.DefaultPattern()(msg))
                {
                    brl.StandardApply(msg);
                    var result = receive.GetList().Result;
                    Assert.IsTrue(result.Any());
                    Assert.IsTrue(result.Count() == 5);
                    Assert.IsTrue(result.Count(c => c == "CD") == 1);
                }
            });
        }
    }

    [TestClass()]
    public class StringParserActorTests
    {
        [TestMethod()]
        public void StringParserActorTest()
        {
            StringParserActor parser = new StringParserActor();
            TestParserActor receive = new TestParserActor();
            TestLauncherActor.Test(() =>
            {
                parser.SendMessage((IActor)receive, "A B C D E");
            });

            TestLauncherActor.Test(() =>
            {
                var result = receive.GetList().Result;
                Assert.IsTrue(result.Any());
                Assert.IsTrue(result.Count() == 5);
                Assert.IsTrue(result.Count(c => c == "C") == 1);
            });
        }
    }

    [TestClass()]
    [Ignore]
    public class ParserServerActorTest
    {
        [TestMethod()]
        public void ParserServerTest()
        {
            ParserServer parser = new ParserServer();
            TestParserActor receive = new TestParserActor();
            TestLauncherActor.Test(() =>
            {
                parser.SendMessage((IActor)receive, "A B C D E");
                var result = receive.GetList().Result;
                Assert.IsTrue(result.Any());
                Assert.IsTrue(result.Count() == 5);
                Assert.IsTrue(result.Count(c => c == "C") == 1);
            });
        }
    }
}
