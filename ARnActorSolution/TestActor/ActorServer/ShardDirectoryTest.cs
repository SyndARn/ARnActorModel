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

        class ShardDirectoryClientTest : BaseActor
        {
            public ShardDirectoryClientTest() : base()
            {
                Become(new Behavior<string>(DoStart)) ;
                this.SendMessage("Start");
            }

            private void DoStart(string msg)
            {
                // find shard in directory
                ConnectActor connect = new ConnectActor(this, ActorServer.GetInstance().FullHost, "KnownShards");
                var data = AsyncReceive(ans => { return ans is IMessageParam<string, ActorTag, IActor>; }) ;
                var res = data.Result as IMessageParam<string, ActorTag, IActor>;
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
            }
        }

        [TestMethod]
        public void TestShardRun()
        {
            TestLauncherActor.Test(() =>
            {
                ActorServer.Start("localhost", 80, new HostRelayActor());
                new ShardDirectoryClientTest(); 
            });
        }
    }
}
