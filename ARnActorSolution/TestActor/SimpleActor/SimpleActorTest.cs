using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestActor;

namespace Actor.Base.Test
{
    [TestClass()]
    public class SimpleActorTest
    {
        [TestMethod()]
        public void SimpleAndReceiveTest()
        {
            TestLauncherActor.Test(() =>
            {
                List<string> list = new List<string>();
                var bhv = new Behavior<IActor, string>((i, s1) => i.SendMessage(i,s1));
                var simple = new SimpleActor(bhv);
                var bhv2 = new Behavior<IActor, string>((i, s2) => list.Add(s2));
                var receiver = new SimpleActor(bhv2);
                simple.SendMessage(receiver,"Simple works");
                Task.Delay(5000).Wait();
                var s = list.FirstOrDefault();
                Assert.AreEqual("Simple works", s);
            });
        }
    }
}
