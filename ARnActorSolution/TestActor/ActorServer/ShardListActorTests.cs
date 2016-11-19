using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestActor
{
    [TestClass()]
    public class ShardListActorTests
    {

        [TestMethod()]
        public void ShardAddTest()
        {
            TestLauncherActor.Test(() =>
            {
                string Shard1 = "Test Shard 1";
                string Shard2 = "Test Shard 2";
                string Shard3 = "Test Shard 3";
                ShardListActor shardList = new ShardListActor();
                shardList.Add(Shard1);
                shardList.Add(Shard2);
                shardList.Add(Shard3);

                var resultList = shardList.GetAll().Result;
                Assert.AreEqual(3, resultList.Count());
                Assert.IsTrue(resultList.Contains(Shard1));
                Assert.IsTrue(resultList.Contains(Shard2));
                Assert.IsTrue(resultList.Contains(Shard3));
            });
        }

        [TestMethod()]
        public void ShardAddRemoveTest()
        {
            TestLauncherActor.Test(() =>
            {
                string Shard1 = "Test Shard 1";
                string Shard2 = "Test Shard 2";
                string Shard3 = "Test Shard 3";
                ShardListActor shardList = new ShardListActor();
                shardList.Add(Shard1);
                shardList.Add(Shard2);
                shardList.Add(Shard3);
                shardList.Remove(Shard2);

                var resultList = shardList.GetAll().Result;
                Assert.AreEqual(2, resultList.Count());
                Assert.IsTrue(resultList.Contains(Shard1));
                Assert.IsFalse(resultList.Contains(Shard2));
                Assert.IsTrue(resultList.Contains(Shard3));
            });
        }

    }
}