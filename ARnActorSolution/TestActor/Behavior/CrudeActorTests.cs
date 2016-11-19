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

        internal class CrudTest : CrudActor<int,string>
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
            TestLauncherActor.Test(() =>
            {
                var act = new CrudTest();
                act.Set(1, "1");
                act.Set(2, "2");
            }
            );
        }

        [TestMethod()]
        public void GetSetTest()
        {
            TestLauncherActor.Test(() =>
            {
                var act = new CrudTest();
                act.Set(1, "1");
                act.Set(2, "2");
                string s = act.Get(1).Result();
                Assert.AreEqual("1", s);
            }
            ) ;
        }

        [TestMethod()]
        public void DeleteTest()
        {
            TestLauncherActor.Test(() =>
            {
                var act = new CrudTest();
                act.Set(1, "1");
                act.Set(2, "2");
                act.Delete(1);
                string s = act.Get(1).Result(2000) ;
                Assert.IsNull(s);
            }
            );
        }

        [TestMethod()]
        public void UpdateTest()
        {
            TestLauncherActor.Test(() =>
            {
                var act = new CrudTest();
                act.Set(1, "1");
                act.Set(2, "2");
                string s = act.Get(1).Result();
                Assert.AreEqual("1", s);
                act.Update(1, "11");
                Assert.AreEqual("11", act.Get(1).Result());
            }
            );
        }
    }
}