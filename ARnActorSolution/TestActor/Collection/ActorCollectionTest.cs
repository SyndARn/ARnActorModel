using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using System.Linq;

namespace TestActor
{
    [TestClass]
    public class ActorCollectionTest
    {
        [TestMethod]
        public void TestActorCollection()
        {
            TestLauncherActor.Test(() =>
                {
                    var collect = new CollectionActor<string>();
                    collect.Add("Test1");
                    collect.Add("Test2");
                    collect.Add("Test3");
                    Assert.IsTrue(collect.Any(t => t == "Test1"));
                    Assert.IsTrue(collect.Contains("Test2"));
                    Assert.IsTrue(collect.First(t => t == "Test3") != null);
                    collect.Remove("Test1");
                    collect.Remove("Test2");
                    collect.Remove("Test3");
                    collect.Add("Test4");
                    collect.Add("Test5");
                    var enumerable = collect.ToList();
                    Assert.AreEqual(2, enumerable.Count);
                });
        }

        [TestMethod]
        public void TestActorCollectionEnumerator()
        {
            TestLauncherActor.Test(() =>
            {
                var collect = new CollectionActor<string>();
                for (int i = 0; i < 100; i++)
                    collect.Add($"Test {i}");
                Assert.AreEqual(100,collect.Count());

                var enumerable = collect.ToList();
                Assert.AreEqual(100, enumerable.Count);

                var query = from col in collect
                            where col.Contains('1')
                            select col;
                Assert.AreEqual(query.Count(), 19);
            });
        }
    }
}
