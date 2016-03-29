using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Server;

namespace Actor.Util
{

    public class ActorAdminServer : BaseActor
    {
        public ActorAdminServer()
        {
            Become(new Behavior<Tuple<IActor, String>>(
                Behavior));
        }

        private void Behavior(Tuple<IActor, String> Data)
        {
            char[] separ = { ' ' };
            var lStrings = Data.Item2.Split(separ, StringSplitOptions.RemoveEmptyEntries);
            var lOrder = lStrings[0];
            var lData = Data.Item2.Replace(lOrder, "").TrimStart();
            switch (lOrder)
            {
                case "Shard":
                    {
                        if (string.IsNullOrEmpty(lData))
                        {
                            ShardRequest req = ShardRequest.CastRequest(this, Data.Item1);
                            SendByName<ShardRequest>.Send(req, "KnownShards");
                        }
                        else
                        {
                            ConnectActor.Connect(this, lData, "KnownShards");
                            Receive(ans => { return ans is Tuple<string, ActorTag, IActor>; }).ContinueWith(
                                ans =>
                                {
                                    var res = ans.Result as Tuple<string, ActorTag, IActor>;
                                    ShardRequest req = ShardRequest.CastRequest(this, Data.Item1);
                                    res.Item3.SendMessage(req);
                                });
                        }
                        break;
                    }
                case "Stat":
                    {
                        ActorStatServer sa = new ActorStatServer();
                        sa.SendMessage(Data.Item1);
                        break;
                    }
                case "GC":
                    {
                        GC.Collect();
                        break;
                    }
                case "AddTask":
                    {
                        // add a task
                        break;
                    }
                case "RemoteEcho":
                    {
                        // have a disco
                        // find EchoServer
                        // send message
                        char[] separ2 = {' '} ;
                        string lHost = lData.Split(separ2)[0] ;
                        string lService = lData.Split(separ2)[1] ;
                        ConnectActor.Connect(this, lHost, lService);
                        var data = Receive(ans => { return ans is Tuple<string, ActorTag, IActor>; }) ;
                        var res = data.Result as Tuple<string, ActorTag, IActor>;
                        // we got remote server adress
                        EchoClientActor aClient = new EchoClientActor();
                        aClient.Connect(res.Item1);
                        aClient.SendMessage("KooKoo");
                        // res.Item3.SendMessage("call from " + this.Tag.Id);
                        break;
                    }
                case "Disco":
                    {
                        // local disco ?
                        if (String.IsNullOrEmpty(lData))
                        {
                            DirectoryActor.GetDirectory().Disco(Data.Item1);
                        }
                        else
                        {
                            new DiscoveryActor(lData);
                            // remote disco
                            //actRemoteSend rem = new actRemoteSend(Data.Item1,lData, "");
                            //rem.SendMessage(new DiscoCommand(rem));
                        }
                        break;
                    }
                case "SendTo":
                    {
                        actRemoteTest.SendString(lData);
                        break;
                    }
                case "RPrint":
                    {
                        char[] separ2 = { ' ' };
                        string lHost = lData.Split(separ2)[0];
                        string lMsg = lData.Split(separ2)[1];
                        ConnectActor.Connect(this, lHost, "RPrint");
                        var data = Receive(ans => { return ans is Tuple<string, ActorTag, IActor>; });
                        var res = data.Result as Tuple<string, ActorTag, IActor>;
                        res.Item3.SendMessage("call  from " + this.Tag.Id);
                        // SendMessageTo("call from " + this.Tag.Id,res.Item3);
                        break;
                    }
            }
        }
    }
}
