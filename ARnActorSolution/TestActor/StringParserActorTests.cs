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
            Become(new Behavior<IActor, string>((a,s) =>
             {
                 fList.Add(s);
             }));
            AddBehavior(new Behavior<IActor>(a =>
            {
                a.SendMessage(fList.AsEnumerable());
            }
            ));
        }
        public IEnumerable<string> GetList()
        {
            var future = new Future<IEnumerable<string>>();
            SendMessage((IActor)future);
            return future.Result();
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
            var launcher = new TestLauncherActor();
            launcher.SendAction(() =>
            {
                parser.SendMessage((IActor)receive, "A B C D E");
                launcher.Finish();
            });
            Assert.IsTrue(launcher.Wait());

            var launcher2 = new TestLauncherActor();
            launcher2.SendAction(() =>
            {
                var result = receive.GetList();
                Assert.IsTrue(result.Any());
                Assert.IsTrue(result.Count() == 5);
                Assert.IsTrue(result.Count(c => c == "C") == 1);
                launcher2.Finish();
            });
            Assert.IsTrue(launcher2.Wait());
        }

        [TestMethod()]
        public void ParserServerTest()
        {
            ParserServer parser = new ParserServer();
            TestParserActor receive = new TestParserActor();
            var launcher = new TestLauncherActor();
            launcher.SendAction(() =>
            {
                parser.SendMessage((IActor)receive, "A B C D E");
                launcher.Finish();
            });
            Assert.IsTrue(launcher.Wait(20000)); // open cover

            var launcher2 = new TestLauncherActor();
            launcher2.SendAction(() =>
            {
                var result = receive.GetList();
                Assert.IsTrue(result.Any());
                Assert.IsTrue(result.Count() == 5);
                Assert.IsTrue(result.Count(c => c == "C") == 1);
                launcher2.Finish();
            });
            Assert.IsTrue(launcher2.Wait(20000)); // open cover
        }        
    }
}