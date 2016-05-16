using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using System.Linq;
using Actor.Util;
using Actor.Server;

namespace TestActor
{
    [TestClass]
    public class ShardDirectoryTest
    {
        private static TestLauncherActor fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new TestLauncherActor();
        }

        class actShardDirectoryClientTest : BaseActor
        {
            public actShardDirectoryClientTest() : base()
            {
                Become(new Behavior<string>(DoStart)) ;
                SendMessage("Start");
            }

            private void DoStart(string msg)
            {
                // find shard in directory
                ConnectActor connect = new ConnectActor(this, ActorServer.GetInstance().FullHost, "KnownShards");
                var data = Receive(ans => { return ans is Tuple<string, ActorTag, IActor>; }) ;
                var res = data.Result as Tuple<string, ActorTag, IActor>;
                var shardDir = res.Item3 ;
                Assert.IsNotNull(shardDir) ;
                ShardRequest req = ShardRequest.CastRequest(this,this) ;
                shardDir.SendMessage(req) ;
                Become(new Behavior<ShardRequest>(WaitAns));
            }

            private void WaitAns(ShardRequest msg)
            {
                // waiting shard answer
                Assert.IsTrue(msg.Data.Count() == 1);
                fLauncher.Finish();
            }
        }

        [TestMethod]
        [Ignore]
        public void TestShardRun()
        {
          ActorServer.Start(this.ToString(), 80, new HostRelayActor());
          fLauncher.SendAction(() => {new actShardDirectoryClientTest(); }) ;
            Assert.IsTrue(fLauncher.Wait(10000));
        }
    }
}
