using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Diagnostics;

namespace Actor.Server
{
    public enum ServerRequest{Connect,Disconnect,Request,Answer,Accept}

    public class ServerMessage<T>
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

    public abstract class ServerBehavior<T> : Behavior<ServerMessage<T>>
    {
        private readonly List<IActor> fActorList = new List<IActor>() ;

        protected ServerBehavior() : base()
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
            if (aMessage == null)
            {
                throw new ActorException("Null message receive in ServerApply");
            }
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
            if (aMessage == null)
            {
                throw new ActorException("Server Behavior, null message encounters");
            }
            if (! fActorList.Contains(aMessage.Client))
            {
                fActorList.Add(aMessage.Client);
            }
            aMessage.Client.SendMessage(new ServerMessage<T>(aMessage.Client, ServerRequest.Accept, default(T)));
        }

        protected void DoDisconnect(ServerMessage<T> aMessage)
        {
            if (aMessage == null)
            {
                throw new ActorException("Server Behavior, null message encounters");
            }
            if (fActorList.Contains(aMessage.Client))
            {
                fActorList.Remove(aMessage.Client);
            }
        }

        protected abstract void DoRequest(ServerMessage<T> aMessage);

        public void SendAnswer(ServerMessage<T> aMessage, T data)
        {
            if (aMessage == null)
            {
                throw new ActorException("Server Behavior, null message encounters");
            }
            if (aMessage.Client == null)
            {
                throw new ActorException("Null client encountered");
            }
            aMessage.Client.SendMessage(new ServerMessage<T>(aMessage.Client, ServerRequest.Answer, data));
        }
    }
}
