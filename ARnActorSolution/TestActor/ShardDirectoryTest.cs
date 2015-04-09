using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using System.Linq;
using Actor.Util;

namespace TestActor
{
    [TestClass]
    public class ShardDirectoryTest
    {
        public static actTestLauncher fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new actTestLauncher();
        }

        class actShardDirectoryClientTest : actActor
        {
            public actShardDirectoryClientTest() : base()
            {
                Become(new bhvBehavior<string>(DoStart)) ;
                SendMessage("Start");
            }

            private void DoStart(string msg)
            {
                // find shard in directory
                actConnect connect = new actConnect(this, ActorServer.GetInstance().FullHost, "KnownShards");
                var data = Receive(ans => { return ans is Tuple<string, actTag, IActor>; }) ;
                var res = data.Result as Tuple<string, actTag, IActor>;
                var shardDir = res.Item3 ;
                Assert.IsNotNull(shardDir) ;
                ShardRequest req = ShardRequest.CastRequest(this,this) ;
                shardDir.SendMessage(req) ;
                Become(new bhvBehavior<ShardRequest>(WaitAns));
            }

            private void WaitAns(ShardRequest msg)
            {
                // waiting shard answer
                Assert.IsTrue(msg.Data.Count() == 1);
                fLauncher.Finish();
            }
        }

        [TestMethod]
        public void TestShardRun()
        {
          ActorServer.Start(this.ToString(), 80);
          fLauncher.SendAction(() => {new actShardDirectoryClientTest(); }) ;
          fLauncher.Wait();
        }
    }
}
