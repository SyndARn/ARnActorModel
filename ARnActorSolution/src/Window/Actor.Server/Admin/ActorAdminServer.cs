using System;
using System.Threading.Tasks;
using Actor.Base;

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
            string[] lStrings = Data.Split(separ, StringSplitOptions.RemoveEmptyEntries);
            string lOrder = lStrings[0];
            string lData = Data.Replace(lOrder, "").TrimStart();
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
                            ReceiveAsync(ans => ans is IMessageParam<string, ActorTag, IActor>).ContinueWith(
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
                        ActorServer.GetInstance().SendMessage(StatServerCommand.Name, asker);
                        // (new StatServerCommand()).Run(asker);
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
                        char[] separ2 = { ' ' };
                        string lHost = lData.Split(separ2)[0];
                        string lService = lData.Split(separ2)[1];
                        ConnectActor.Connect(this, lHost, lService);
                        Task<object> data = ReceiveAsync(ans => ans is IMessageParam<string, ActorTag, IActor>);
                        var res = data.Result as IMessageParam<string, ActorTag, IActor>;
                        // we got remote server adress
                        var aClient = new EchoClientActor();
                        aClient.Connect(res.Item1);
                        aClient.SendMessage("KooKoo");
                        // res.Item3.SendMessage("call from " + this.Tag.Id);
                        break;
                    }
                case "Disco":
                    {
                        ActorServer.GetInstance().SendMessage(DiscoServerCommand.Name, (IActor)asker, lData);
                       // (new DiscoServerCommand()).Run(asker, lData);
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
                        Task<object> data = ReceiveAsync(ans => ans is IMessageParam<string, ActorTag, IActor>);
                        var res = data.Result as IMessageParam<string, ActorTag, IActor>;
                        res.Item3.SendMessage("call  from " + this.Tag.Key());
                        // SendMessageTo("call from " + this.Tag.Id,res.Item3);
                        break;
                    }
            }
        }
    }
}
