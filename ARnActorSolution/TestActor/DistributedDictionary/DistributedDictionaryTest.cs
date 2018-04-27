using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using System.Linq;

namespace TestActor
{
    [TestClass]
    public class DistributedDictionaryTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestDistributedDictionary()
        {
            TestLauncherActor.Test(TestContext,
                () =>
                {
                    var dic = new DistributedDictionaryActor<int, string>();
                    foreach(var i in Enumerable.Range(1,1000))
                    {
                        dic.AddKeyValue(i, i.ToString() + " Test");
                    }
                    // find key 3
                    var future = dic.GetKeyValue(3);
                    var result = future.Result();
                    Assert.AreEqual(result.Item1, true);
                    Assert.AreEqual(result.Item2 ,3);
                    Assert.AreEqual(result.Item3, "3 Test");
                    // don't find key -1
                    var noFuture = dic.GetKeyValue(-1);
                    var noResult = noFuture.Result(1000);
                    Assert.IsNotNull(noResult);
                    Assert.AreEqual(noResult.Item1,false);
                });
        }
    }
}
