using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using System.Collections.Concurrent;
using Actor.Base;
using System.Linq;

namespace TestActor
{
    [TestClass]
    public class ActorCollectionTest
    {

        TestLauncherActor fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new TestLauncherActor();
        }


        [TestMethod]
        public void TestActorCollection()
        {
            fLauncher.SendAction(() =>
                {
                    var collect = new CollectionActor<string>();
                    collect.Add("Test1");
                    collect.Add("Test2");
                    collect.Add("Test3");
                    Assert.IsTrue(collect.Any(t => t == "Test1"));
                    Assert.IsTrue(collect.Contains("Test2"));
                    Assert.IsTrue(collect.Where(t => t == "Test3").First() != null);
                    collect.Remove("Test1");
                    //Assert.IsFalse(collect.Exists("Test1").Result);
                    collect.Remove("Test2");
                    //Assert.IsFalse(collect.Exists("Test2").Result);
                    collect.Remove("Test3");
                    //Assert.IsFalse(collect.Exists("Test3").Result);
                    collect.Add("Test4");
                    collect.Add("Test5");
                    var enumerable = collect.ToList();
                    Assert.AreEqual(2, enumerable.Count);
                    fLauncher.Finish();
                });
            Assert.IsTrue(fLauncher.Wait());
        }

        [TestMethod]
        public void TestActorCollectionEnumerator()
        {
            fLauncher.SendAction(() =>
            {
                var collect = new CollectionActor<string>();
                for (int i = 0; i < 100; i++)
                    collect.Add(string.Format("Test {0}", i));
                Assert.AreEqual(100,collect.Count());
                // try to enum
                var enumerable = collect.ToList();
                Assert.AreEqual(100, enumerable.Count);
                // try a query
                var query = from col in collect
                            where col.Contains('1')
                            select col;
                Assert.AreEqual(query.Count(), 19);
                fLauncher.Finish();
            });
            Assert.IsTrue(fLauncher.Wait());
        }
    }
}
