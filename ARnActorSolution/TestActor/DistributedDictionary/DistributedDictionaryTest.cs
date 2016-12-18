using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using System.Linq;

namespace TestActor
{
    [TestClass]
    public class DistributedDictionaryTest
    {

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void TestDistributedDictionary()
        {
            TestLauncherActor.Test(TestContext,
                () =>
                {
                    var dic = new DistributedDictionaryActor<int, string>();
                    foreach(var i in Enumerable.Range(1,1000))
                    {
                        dic.Add(i, i.ToString() + " Test");
                    }
                    // find key 3
                    var future = dic.Get(3);
                    var result = future.Result();
                    Assert.AreEqual(result.Item1 ,3);
                    Assert.AreEqual(result.Item2, "3 Test");
                    // don't find key -1
                    var noFuture = dic.Get(-1);
                    var noResult = noFuture.Result(1000);
                    Assert.IsNull(noResult);
                });
        }
    }
}
