using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{
    public class actEchoServer : actActor 
    {
        public actEchoServer()
            : base()
        {
            actHostDirectory.Register(this);
            actDirectory.GetDirectory().Register(this, "EchoServer");
            Become(new bhvEchoServer());
        }
    }

    public class actEchoClient : actActor
    {
        private bhvEchoClient aClient;
        public actEchoClient()
            : base()
        {
            aClient = new bhvEchoClient();
        }

        public void Connect(string ServerName)
        {
            Become(new bhvBehavior<Tuple<string,string>>(t => { return t.Item1 == "Connect"; }, DoConnect));
            this.SendMessageTo(Tuple.Create("Connect",ServerName));
        }

        protected void DoConnect(Tuple<string,string> msgcon)
        {
                // find in directory
                actDirectory.GetDirectory().Find(this, msgcon.Item2);
                Receive(ask => { return (ask is Tuple<actDirectory.DirectoryRequest, IActor>); }).ContinueWith(
                    r =>
                    {
                        Tuple<actDirectory.DirectoryRequest, IActor> ans = (Tuple<actDirectory.DirectoryRequest, IActor>)(r.Result);
                        if (ans.Item2 != null)
                        {
                            actSendByName<string>.SendByName("Server found", "Console");
                            SendMessageTo(new ServerMessage<string>(this, ServerRequest.Connect, default(string)),ans.Item2);
                            Receive(m => (m is ServerMessage<string>) && (((ServerMessage<string>)m).Request.Equals(ServerRequest.Accept))).ContinueWith(
                                (c) =>
                                {
                                    actSendByName<string>.SendByName("Client connected", "Console");
                                    aClient.Connect(ans.Item2);
                                    Become(aClient);
                                });
                        }
                        else
                        {
                            Console.WriteLine("Retry");
                            SendMessageTo(msgcon);
                            // Become(null);
                        }
                    });
            // repeat message
        }

        public void SendMessage(string s)
        {
            SendMessageTo(new ServerMessage<string>(this, ServerRequest.Request, s));
        }
        //public void Disconnect()
        //{
        //    aClient.Disconnect();
        //}
    }

    class bhvEchoServer : bhvServer<string>
    {
        protected override void DoRequest(ServerMessage<string> aMessage)
        {
            // echo to console
            actSendByName<string>.SendByName(
                "server receive " + aMessage.Data,"Console");
            // back to client
            SendAnswer(aMessage,aMessage.Data) ;
        }
    }


    class bhvEchoClient : bhvClient<string>
    {
       protected override void ReceiveAnswer(ServerMessage<string> aMessage)
        {
            // echo to console
            actSendByName<string>.SendByName(
                "client receive " + aMessage.Data,"Console");
        }
    }
}
