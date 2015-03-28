﻿using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{

    public class ActorAdminServer : actActor
    {
        public ActorAdminServer()
        {
            Become(new bhvBehavior<Tuple<IActor, String>>(
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
                            var byName = new actSendByName<ShardRequest>();
                            SendMessageTo(Tuple.Create("KnownShards", req), byName);
                        }
                        else
                        {
                            actConnect connect = new actConnect(this, lData, "KnownShards");
                            Receive(ans => { return ans is Tuple<string, actTag, IActor>; }).ContinueWith(
                                ans =>
                                {
                                    var res = ans.Result as Tuple<string, actTag, IActor>;
                                    ShardRequest req = ShardRequest.CastRequest(this, Data.Item1);
                                    SendMessageTo(req, res.Item3);
                                });
                        }
                        break;
                    }
                case "Stat":
                    {
                        ActorStatServer sa = new ActorStatServer();
                        SendMessageTo(
                            Data.Item1, sa);
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
                        actConnect connect = new actConnect(this, lHost, lService);
                        var data = Receive(ans => { return ans is Tuple<string, actTag, IActor>; }) ;
                        var res = data.Result as Tuple<string, actTag, IActor>;
                        // we got remote server adress
                        actEchoClient aClient = new actEchoClient();
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
                            actDirectory.GetDirectory().Disco(Data.Item1);
                        }
                        else
                        {
                            new actDiscovery(lData);
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
                        actConnect connect = new actConnect(this, lHost, "RPrint");
                        var data = Receive(ans => { return ans is Tuple<string, actTag, IActor>; });
                        var res = data.Result as Tuple<string, actTag, IActor>;
                        SendMessageTo("call  from " + this.Tag.Id, res.Item3);
                        // SendMessageTo("call from " + this.Tag.Id,res.Item3);
                        break;
                    }
            }
        }
    }
}