using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actor.Server
{
    public class ActorAdminServer : BaseActor
    {
        public ActorAdminServer()
        {
            Become(new Behavior<IActor, string>(
                Behavior));
        }

        private void Behavior(IActor asker, string Data)
        {
            char[] separ = { ' ' };
            var lStrings = Data.Split(separ, StringSplitOptions.RemoveEmptyEntries);
            var lOrder = lStrings[0];
            var lData = Data.Replace(lOrder, "").TrimStart();
            switch (lOrder)
            {
                case "Shard":
                    {
                        if (string.IsNullOrEmpty(lData))
                        {
                            ShardRequest req = ShardRequest.CastRequest(this, asker);
                            SendByName<ShardRequest>.Send(req, "KnownShards");
                        }
                        else
                        {
                            ConnectActor.Connect(this, lData, "KnownShards");
                            Receive(ans => ans is IMessageParam<string, ActorTag, IActor>).ContinueWith(
                                ans =>
                                {
                                    var res = ans.Result as IMessageParam<string, ActorTag, IActor>;
                                    ShardRequest req = ShardRequest.CastRequest(this, asker);
                                    res.Item3.SendMessage(req);
                                });
                        }
                        break;
                    }
                case "Stat":
                    {
                        (new StatServerCommand()).Run(asker);
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
                        var data = Receive(ans => ans is IMessageParam<string, ActorTag, IActor>) ;
                        var res = data.Result as IMessageParam<string, ActorTag, IActor>;
                        // we got remote server adress
                        EchoClientActor aClient = new EchoClientActor();
                        aClient.Connect(res.Item1);
                        aClient.SendMessage("KooKoo");
                        // res.Item3.SendMessage("call from " + this.Tag.Id);
                        break;
                    }
                case "Disco":
                    {
                        (new DiscoServerCommand()).Run(asker, lData);
                        break;
                    }
                case "SendTo":
                    {
                        RemoteNetActor.SendString(lData);
                        break;
                    }
                case "RPrint":
                    {
                        char[] separ2 = { ' ' };
                        string lHost = lData.Split(separ2)[0];
                        string lMsg = lData.Split(separ2)[1];
                        ConnectActor.Connect(this, lHost, "RPrint");
                        var data = Receive(ans => ans is IMessageParam<string, ActorTag, IActor>);
                        var res = data.Result as IMessageParam<string, ActorTag, IActor>;
                        res.Item3.SendMessage("call  from " + this.Tag.Key());
                        // SendMessageTo("call from " + this.Tag.Id,res.Item3);
                        break;
                    }
            }
        }
    }
}
