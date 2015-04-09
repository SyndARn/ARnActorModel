using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    [Serializable]
    public class ShardRequest
    {
        public string RequestType {get;private set;} // Ask or Answer
        public IActor Sender {get ; private set;}
        public IActor Target {get ; private set;}
        public IEnumerable<string> Data {get;private set;}
        public ShardRequest(){} 
        public static ShardRequest CastRequest(IActor aSender, IActor aTarget)
        {
            var req = new ShardRequest() ;
            req.RequestType = "Ask" ;
            req.Sender = aSender ;
            req.Target = aTarget ;
            return req ;
        }
        public ShardRequest CastAnswer(IEnumerable<string> someData)
        {
            var req = new ShardRequest() ;
            req.RequestType = "Answer" ;
            req.Sender = this.Sender ;
            req.Target = this.Target ;
            req.Data = new List<string>(someData) ;
            return req ;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder() ;
            foreach(var s in Data)
                sb.AppendLine(s) ;
            return sb.ToString();
        }
    }

    public class actShardDirectory : actActor
    {
        private Dictionary<string,string> fShardList;
        public actShardDirectory()
            : base()
        {
            fShardList = new Dictionary<string,string>();
            fShardList.Add("LocalHost",ActorServer.GetInstance().FullHost);
            actDirectory.GetDirectory().Register(this, "KnownShards");
            actHostDirectory.Register(this);
            Become(new bhvBehavior<ShardRequest>(
                t => t is ShardRequest,
                DoProcessShardRequest)) ;
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
                case "Answer" :
                    {
                        foreach(var item in msg.Data)
                            fShardList.Add(item,item);
                        break;
                    }
                default: break;
            }
        }
    }

    public class ShardList : actAction<string>
    {
        private HashSet<string> fShardList = new HashSet<string>();
        public ShardList() : base() { }
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
        public async Task<IEnumerable<string>> GetAll()
        {
            SendAction(DoGetAll);
            return await Receive(t => { return t is IEnumerable<string>; }) as IEnumerable<string> ;
        }
        private void DoGetAll()
        {
            SendMessage(fShardList.AsEnumerable<string>());
        }
    }


}
