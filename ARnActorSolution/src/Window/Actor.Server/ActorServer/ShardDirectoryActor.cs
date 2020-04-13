using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Server
{
    [Serializable]
    public class ShardRequest
    {
        public string RequestType { get; private set; } // Ask or Answer
        public IActor Sender { get; private set; }
        public IActor Target { get; private set; }
        public IEnumerable<string> Data { get; private set; }

        public ShardRequest() { }

        public static ShardRequest CastRequest(IActor aSender, IActor aTarget)
        {
            var req = new ShardRequest()
            {
                RequestType = "Ask",
                Sender = aSender,
                Target = aTarget
            };
            return req;
        }

        public ShardRequest CastAnswer(IEnumerable<string> someData)
        {
            var req = new ShardRequest()
            {
                RequestType = "Answer",
                Sender = this.Sender,
                Target = this.Target,
                Data = new List<string>(someData)
            };
            return req;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var s in Data)
                sb.AppendLine(s);
            return sb.ToString();
        }
    }

    public class ShardDirectoryActor : BaseActor
    {
        private readonly Dictionary<string, string> fShardList;

        public ShardDirectoryActor(ActorServer actorServer)
            : base()
        {
            fShardList = new Dictionary<string, string>
            {
                { "LocalHost",actorServer.FullHost }
            };
            DirectoryActor.GetDirectory().Register(this, "KnownShards");
            HostDirectoryActor.Register(this);
            Become(new Behavior<ShardRequest>(
                t => t is ShardRequest,
                DoProcessShardRequest));
        }

        private void DoProcessShardRequest(ShardRequest msg)
        {
            switch (msg.RequestType)
            {
                case "Ask":
                    {
                        var ans = msg.CastAnswer(fShardList.Values.AsEnumerable<string>());
                        msg.Target.SendMessage(ans);
                        break;
                    }
                case "Answer":
                    {
                        foreach (var item in msg.Data)
                            fShardList.Add(item, item);
                        break;
                    }
                default: break;
            }
        }
    }

    public class ShardListActor : ActionActor<string>
    {
        private readonly HashSet<string> fShardList = new HashSet<string>();

        public ShardListActor() : base()
        {
            AddBehavior(new Behavior<IFuture>(DoGetAll));
        }

        public void Add(string aShardName)
        {
            SendAction(DoSend, aShardName);
        }

        private void DoSend(string aShardName)
        {
            fShardList.Add(aShardName);
        }

        public void Remove(string aShardName)
        {
            SendAction(DoRemove, aShardName);
        }

        private void DoRemove(string aShardName)
        {
            fShardList.Remove(aShardName);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public async Task<IEnumerable<string>> GetAll()
        {
            IFuture<IEnumerable<string>> future = new Future<IEnumerable<string>>();
            SendMessage(future);
            return await future.ResultAsync().ConfigureAwait(false);
        }

        private void DoGetAll(IFuture future)
        {
            IEnumerable<string> list = fShardList.ToList().AsEnumerable();
            future.SendMessage(list);
        }
    }
}
