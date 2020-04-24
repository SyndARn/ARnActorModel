using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestActor
{
    [TestClass()]
    public class DictionaryActorTests
    {
        [TestMethod()]
        public void DictionaryActorTest()
        {
            TestLauncherActor.Test(() =>
            {
                var act = new DictionaryActor<string, int>();
                Assert.IsNotNull(act);
            });
        }

        [TestMethod()]
        public void AddKVTest()
        {
            TestLauncherActor.Test(() =>
            {
                var act = new DictionaryActor<string, int>();
                Assert.IsNotNull(act);
                act.AddKeyValue("1", 1);
                act.AddKeyValue("2", 2);
            });
        }

        [TestMethod()]
        public void GetKeyValueTest()
        {
            TestLauncherActor.Test(() =>
            {
                var act = new DictionaryActor<string, int>();
                Assert.IsNotNull(act);
                act.AddKeyValue("1", 1);
                act.AddKeyValue("2", 2);
                var r1 = act.GetKeyValue("1");
                var r2 = act.GetKeyValue("2");
                var r3 = act.GetKeyValue("3");
                var f1 = r1.Result();
                var f2 = r2.Result();
                Assert.IsTrue(f1.Item1);
                Assert.IsTrue(f2.Item1);
                Assert.AreEqual("2", f2.Item2);
                Assert.AreEqual(2, f2.Item3);
                Assert.IsFalse(r3.Result().Item1);
            });
        }

        [TestMethod()]
        public void RemoveKTest()
        {
            TestLauncherActor.Test(() =>
            {
                var act = new DictionaryActor<string, int>();
                Assert.IsNotNull(act);
                act.AddKeyValue("1", 1);
                act.AddKeyValue("2", 2);
                act.RemoveKey("1");
                var r1 = act.GetKeyValue("2");
                Assert.AreEqual(2, r1.Result().Item3);
            });
        }

        [TestMethod()]
        public void AsEnumerableTest()
        {
            TestLauncherActor.Test(() =>
            {
                var act = new DictionaryActor<string, int>();
                Assert.IsNotNull(act);
                act.AddKeyValue("1", 1);
                act.AddKeyValue("2", 2);
                act.AddKeyValue("3", 3);
                act.AddKeyValue("4", 4);
                var future = act.AsEnumerable();
                var result = future.Result();
                var first = result.First();
                Assert.AreEqual("[1, 1]", first.ToString());
            });
        }
    }
}