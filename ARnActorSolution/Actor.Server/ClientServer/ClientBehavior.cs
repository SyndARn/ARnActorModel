using Actor.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Server
{

    public abstract class ClientBehavior<T> : Behavior<ServerMessage<T>>
    {
        private IActor fServer = null;
        protected ClientBehavior() : base()
        {
            Pattern = t => { return true; };
            Apply = DispatchAnswer;
        }

        protected void DispatchAnswer(ServerMessage<T> aMessage)
        {
            if (aMessage == null)
            {
                throw new ActorException("Null message receive in ServerBehavior");
            }
            switch (aMessage.Request)
            {
                case ServerRequest.Answer: { ReceiveAnswer(aMessage); break; };
                case ServerRequest.Request: { SendRequest(aMessage); break; };
                default: { Debug.WriteLine("bad request receive"); break; };
            }
        }

        protected abstract void ReceiveAnswer(ServerMessage<T> aMessage);

        public void Connect(IActor aServer)
        {
            fServer = aServer;
        }

        protected void SendRequest(ServerMessage<T> aMessage)
        {
            if (aMessage == null)
            {
                throw new ActorException("Null message receive in SendRequest");
            }
            fServer.SendMessage(new ServerMessage<T>(LinkedTo.LinkedActor, ServerRequest.Request, aMessage.Data));
        }

    }

}
