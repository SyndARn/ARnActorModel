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
    public class CrudeActorTests
    {
        TestLauncherActor fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new TestLauncherActor();
        }

        class CrudTest : CrudActor<int,string>
        {

        }

        [TestMethod()]
        public void CrudeNewTest()
        {
            var act = new CrudTest();
            Assert.IsNotNull(act);
        }

        [TestMethod()]
        public void SetTest()
        {
            fLauncher.SendAction(() =>
            {
                var act = new CrudTest();
                act.Set(1, "1");
                act.Set(2, "2");
                fLauncher.Finish();
            }
            );
            Assert.IsTrue(fLauncher.Wait());
        }

        [TestMethod()]
        public void GetSetTest()
        {
            fLauncher.SendAction( () =>
            {
                var act = new CrudTest();
                act.Set(1, "1");
                act.Set(2, "2");
                string s = act.Get(1).Result();
                Assert.AreEqual("1", s);
                fLauncher.Finish();
            }
            ) ;
            Assert.IsTrue(fLauncher.Wait());
        }

        [TestMethod()]
        public void DeleteTest()
        {
            fLauncher.SendAction(() =>
            {
                var act = new CrudTest();
                act.Set(1, "1");
                act.Set(2, "2");
                act.Delete(1);
                string s = act.Get(1).Result(2000) ;
                Assert.IsNull(s);
                fLauncher.Finish();
            }
            );
            Assert.IsTrue(fLauncher.Wait());
        }

        [TestMethod()]
        public void UpdateTest()
        {
            fLauncher.SendAction(() =>
            {
                var act = new CrudTest();
                act.Set(1, "1");
                act.Set(2, "2");
                string s = act.Get(1).Result();
                Assert.AreEqual("1", s);
                act.Update(1, "11");
                Assert.AreEqual("11", act.Get(1).Result());
                fLauncher.Finish();
            }
            );
            Assert.IsTrue(fLauncher.Wait());
        }
    }
}