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
        public void GetKVTest()
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
                Assert.IsTrue(r1.Result().Item1);
                Assert.IsTrue(r2.Result().Item1);
                Assert.AreEqual("2",r2.Result().Item2) ;
                Assert.AreEqual(2, r2.Result().Item3);
                Assert.IsFalse(r3.Result().Item1);
            });
        }
    }
}