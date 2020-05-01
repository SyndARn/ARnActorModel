using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Globalization;
using TestActor;

namespace Actor.Util.Test
{
    [TestClass()]
    public class EnumerableActorTests
    {
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
                sum += int.Parse(item, CultureInfo.InvariantCulture);
            }

            Assert.AreEqual(expected, sum);
        }

        [TestMethod()]
        public void EnumerableActorTest()
        {
            TestLauncherActor.Test(() =>
            {
                var act = new EnumerableActor<string>();
                Assert.IsNotNull(act);
            });
        }

        [TestMethod()]
        public void AddElementTest()
        {
            TestLauncherActor.Test(() =>
            {
                var act = new EnumerableActor<string>
                {
                    "test"
                };
            });
        }

        [TestMethod()]
        public void RemoveElementTest()
        {
            TestLauncherActor.Test(() =>
            {
                var act = new EnumerableActor<string>
                {
                    "test"
                };
                act.Remove("test");
            });
        }

        [TestMethod()]
        public void GetEnumeratorTest()
        {
            TestLauncherActor.Test(() =>
            {
                var act = new EnumerableActor<string>
                {
                    "test1",
                    "test2"
                };
                var list = new List<string>();
                foreach(var item in act)
                {
                    list.Add(item);
                }
                Assert.AreEqual(2, list.Count);
                Assert.AreEqual(2, act.Count);
            });
        }

        [TestMethod()]
        public void StressEnumeratorWithLinqTest()
        {
            TestLauncherActor.Test(() =>
            {
                var act = new EnumerableActor<string>();

                foreach (var item in Enumerable.Range(0, 100))
                {
                    act.Add($"{item}");
                }
                Assert.AreEqual(100, act.Count());
                var query =
                    from s in act
                    where s.StartsWith("1",StringComparison.InvariantCulture)
                    select s;
                Assert.AreEqual(11, query.Count());
                Assert.IsTrue(query.Contains("11"));
            });
        }
    }
}