using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using System.Collections.Generic;
using System.Linq;
using TestActor;

namespace Actor.Util.Test
{
        internal class ExtensionTestActor : BaseActor
    {
        private readonly List<string> _list = new List<string>();
        public ExtensionTestActor() : base()
        {
            Become(new Behavior<string>(s => _list.Add(s)));
            AddBehavior(new Behavior<string,string>((s1,s2) => _list.Add(s2)));
            AddBehavior(new Behavior<string, string,string>((s1, s2, s3) => _list.Add(s3)));
            AddBehavior(new Behavior<string, string, string, string>((s1, s2, s3, s4) => _list.Add(s4)));
            AddBehavior(new Behavior<IActor>(a => a.SendMessage(_list.AsEnumerable())));
        }
    }

    [TestClass]
    public class IActorExtensionTest
    {
        [TestMethod]
        public void TestActorExtension()
        {
            TestLauncherActor.Test(() =>
            {
                var receiver = new ExtensionTestActor();
                var future = new Future<IEnumerable<string>>();
                receiver.SendMessage("test1");
                receiver.SendMessage("test1","test2");
                receiver.SendMessage("test1", "test2","test3");
                receiver.SendMessage("test1", "test2", "test3", "test4");
                receiver.SendMessage(future);
                var result = future.Result();
                Assert.IsNotNull(result);
                Assert.AreEqual(4, result.Count());
                Assert.IsTrue(result.Contains("test4"));
            });
        }
    }
}
