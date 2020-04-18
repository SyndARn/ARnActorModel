using System.Collections.Generic;
using Actor.Base;

namespace Actor.Server
{
    public abstract class ServerBehavior<T> : Behavior<ServerMessage<T>>
    {
        private readonly List<IActor> fActorList = new List<IActor>();

        protected ServerBehavior() : base()
        {
            Pattern = ServerPattern;
            Apply = ServerApply;
        }

        public bool ServerPattern(ServerMessage<T> aMessage) => true;

        public void ServerApply(ServerMessage<T> aMessage)
        {
            if (aMessage == null)
            {
                throw new ActorException("Null message receive in ServerApply");
            }
            switch (aMessage.Request)
            {
                case ServerRequest.Connect: DoConnect(aMessage); break;
                case ServerRequest.Disconnect: DoDisconnect(aMessage); break;
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
            if (!fActorList.Contains(aMessage.Client))
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
