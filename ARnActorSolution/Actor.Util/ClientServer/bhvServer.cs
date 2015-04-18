using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Diagnostics;

namespace Actor.Util
{
    public enum ServerRequest{Connect,Disconnect,Request,Answer,Accept}

    class ServerMessage<T>
    {
        public ServerMessage(IActor aClient, ServerRequest aRequest, T aData)
        {
            Request = aRequest;
            Data = aData;
            Client = aClient;
        }
        public IActor Client { get; set; }
        public ServerRequest Request { get; set; }
        public T Data { get; set; }
    }

    abstract class bhvServer<T> : bhvBehavior<ServerMessage<T>>
    {
        private List<IActor> fActorList = new List<IActor>() ;

        public bhvServer() : base()
        {
            Pattern = ServerPattern;
            Apply = ServerApply;
        }

        public bool ServerPattern(ServerMessage<T> aMessage)
        {
            return true ;
        }

        public void ServerApply(ServerMessage<T> aMessage)
        {
            switch (aMessage.Request)
            {
                case ServerRequest.Connect: DoConnect(aMessage);  break;
                case ServerRequest.Disconnect: DoDisconnect(aMessage);  break;
                case ServerRequest.Request: DoRequest(aMessage); break;
                default: break; 
            }
        }

        protected void DoConnect(ServerMessage<T> aMessage)
        {
            if (! fActorList.Contains(aMessage.Client))
            {
                fActorList.Add(aMessage.Client);
            }
            aMessage.Client.SendMessage(new ServerMessage<T>(aMessage.Client, ServerRequest.Accept, default(T)));
        }

        protected void DoDisconnect(ServerMessage<T> aMessage)
        {
            if (fActorList.Contains(aMessage.Client))
            {
                fActorList.Remove(aMessage.Client);
            }
        }

        protected abstract void DoRequest(ServerMessage<T> aMessage);

        public void SendAnswer(ServerMessage<T> aMessage, T data)
        {
            aMessage.Client.SendMessage(new ServerMessage<T>(aMessage.Client, ServerRequest.Answer, data));
        }
    }


    abstract class bhvClient<T> : bhvBehavior<ServerMessage<T>>
    {
        private IActor fServer = null ;
        public bhvClient() : base ()        
        {
            Pattern = t => {return true;};
            Apply = DispatchAnswer;
        }

        protected void DispatchAnswer(ServerMessage<T> aMessage)
        {
            switch(aMessage.Request)
            {
                case ServerRequest.Answer: { ReceiveAnswer(aMessage); break; };
                case ServerRequest.Request: { SendRequest(aMessage); break; };
                default: { Debug.WriteLine("bad request receive"); break; };
            }
        }

        protected abstract void ReceiveAnswer(ServerMessage<T> aMessage) ;

        public void Connect(IActor aServer)
        {
            fServer = aServer ;
        }

        protected void SendRequest(ServerMessage<T> aMessage)
        {
            fServer.SendMessage(new ServerMessage<T>(LinkedTo.LinkedActor, ServerRequest.Request, aMessage.Data));
        }

    }
}
