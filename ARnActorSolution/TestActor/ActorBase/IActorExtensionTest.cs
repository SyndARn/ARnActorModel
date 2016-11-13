using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using Actor.Base;
using System.Collections.Generic;
using System.Linq;

namespace TestActor
{

    class ExtensionTestActor : BaseActor
    {
        private List<string> fList = new List<string>();
        public ExtensionTestActor() : base()
        {
            Become(new Behavior<string>(s => fList.Add(s)));
            AddBehavior(new Behavior<string,string>((s1,s2) => fList.Add(s2)));
            AddBehavior(new Behavior<string, string,string>((s1, s2, s3) => fList.Add(s3)));
            AddBehavior(new Behavior<string, string, string, string>((s1, s2, s3, s4) => fList.Add(s4)));
            AddBehavior(new Behavior<IActor>(a => a.SendMessage(fList.AsEnumerable())));
        }
    }

    [TestClass]
    public class IActorExtensionTest
    {
        [TestMethod]
        public void TestActorExtension()
        {
            var launcher = new TestLauncherActor();
            launcher.SendAction(() =>
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
                launcher.Finish();
            });
            Assert.IsTrue(launcher.Wait());
        }
    }
}
