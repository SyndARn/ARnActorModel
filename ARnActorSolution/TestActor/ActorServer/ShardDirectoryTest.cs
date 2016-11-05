using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using System.Linq;
using Actor.Util;
using Actor.Server;
using System.Configuration;

namespace TestActor
{
    [TestClass]
    [Ignore]
    public class ShardDirectoryTest
    {

        class actShardDirectoryClientTest : BaseActor
        {
            public actShardDirectoryClientTest() : base()
            {
                Become(new Behavior<IFuture<ShardRequest>>(DoStart)) ;
            }

            private void DoStart(IFuture<ShardRequest> msg)
            {
                // find shard in directory
                ConnectActor connect = new ConnectActor(this, ActorServer.GetInstance().FullHost, "KnownShards");
                var data = Receive(ans => { return ans is Tuple<string, ActorTag, IActor>; }) ;
                var res = data.Result as Tuple<string, ActorTag, IActor>;
                var shardDir = res.Item3 ;
                Assert.IsNotNull(shardDir) ;
                ShardRequest req = ShardRequest.CastRequest(this,msg) ;
                shardDir.SendMessage(req) ;
                Become(new NullBehavior());
            }

        }

        [TestMethod]
        public void TestShardRun()
        {
            TestLauncherActor.Test(() =>
            {
                ConfigurationManager.AppSettings["ListenerService"] = "MemoryListenerService";
                ConfigurationManager.AppSettings["SerializeService"] = "NetDataContractSerializeService";
                ActorServer.Start(this.ToString(), 80, new HostRelayActor());
                IFuture<ShardRequest> future = new Future<ShardRequest>();
                var shard = new actShardDirectoryClientTest();
                shard.SendMessage(future);
                var result1 = future.Result();
                Assert.IsTrue(result1.Data.Count() == 1);
            });
        }
    }
}
