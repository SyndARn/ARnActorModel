using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Globalization;

namespace TestActor
{
    [TestClass()]
    public class EnumerableActorTests
    {

        private TestLauncherActor fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new TestLauncherActor();
        }

        [TestMethod()]
        public void QueryiableTest()
        {
            var col = new EnumerableActor<int>();

            int expected = 0;
            for (int i = 0; i <= 100; i++)
            {
                col.Add(i);
                expected += i;
            }

            var map = from item in col
                      select item.ToString() ;

            var queryActor = map.AsActorQueryiable();

            int sum = 0;
            foreach (var item in queryActor)
            {
                sum = sum + int.Parse(item,CultureInfo.InvariantCulture);
            }

            Assert.AreEqual(expected, sum);
        }

        [TestMethod()]
        public void EnumerableActorTest()
        {
            fLauncher.SendAction(() =>
            {
                var act = new EnumerableActor<string>();
                Assert.IsNotNull(act);
                fLauncher.Finish();
            });
            Assert.IsTrue(fLauncher.Wait());
        }

        [TestMethod()]
        public void AddElementTest()
        {
            fLauncher.SendAction(() =>
            {
                var act = new EnumerableActor<string>();
                act.Add("test");
                fLauncher.Finish();
            });
            Assert.IsTrue(fLauncher.Wait());
        }

        [TestMethod()]
        public void RemoveElementTest()
        {
            fLauncher.SendAction(() =>
            {
                var act = new EnumerableActor<string>();
                act.Add("test");
                act.Remove("test");
                fLauncher.Finish();
            });
            Assert.IsTrue(fLauncher.Wait());
        }

        [TestMethod()]
        public void GetEnumeratorTest()
        {
            fLauncher.SendAction(() =>
            {
                var act = new EnumerableActor<string>();
                act.Add("test1");
                act.Add("test2");
                var list = new List<string>();
                foreach(var item in act)
                {
                    list.Add(item);
                }
                Assert.AreEqual(2, list.Count);
                Assert.AreEqual(2, act.Count());
                fLauncher.Finish();
            });
            Assert.IsTrue(fLauncher.Wait());
        }

        [TestMethod()]
        public void StressEnumeratorWithLinqTest()
        {
            TestLauncherActor.Test(() =>
            {
                var act = new EnumerableActor<string>();

                foreach (var item in Enumerable.Range(0, 100))
                {
                    act.Add(item.ToString());
                }
                Assert.AreEqual(100, act.Count());
                var query =
                    from s in act
                    where s.StartsWith("1")
                    select s;
                Assert.AreEqual(11, query.Count());
                Assert.IsTrue(query.Contains("11"));
            });
        }
    }
}