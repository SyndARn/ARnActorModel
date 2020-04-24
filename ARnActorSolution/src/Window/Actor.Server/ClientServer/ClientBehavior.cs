using Actor.Base;
using System.Diagnostics;

namespace Actor.Server
{
    public abstract class ClientBehavior<T> : Behavior<ServerMessage<T>>
    {
        const string MessageNullInServerBehavior = "Null message receive in ServerBehavior";
        const string MessageNullInSendRequest = "Null message receive in SendRequest";

        private IActor fServer = null;

        protected ClientBehavior() : base()
        {
            Pattern = t => true;
            Apply = DispatchAnswer;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        protected void DispatchAnswer(ServerMessage<T> aMessage)
        {
            if (aMessage == null)
            {
                throw new ActorException(MessageNullInServerBehavior);
            }
            switch (aMessage.Request)
            {
                case ServerRequest.Answer: { ReceiveAnswer(aMessage); break; }
                case ServerRequest.Request: { SendRequest(aMessage); break; }
                default: { Debug.WriteLine("bad request receive"); break; }
            }
        }

        protected abstract void ReceiveAnswer(ServerMessage<T> aMessage);

        public void Connect(IActor aServer)
        {
            fServer = aServer;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        protected void SendRequest(ServerMessage<T> aMessage)
        {
            if (aMessage == null)
            {
                throw new ActorException(MessageNullInSendRequest);
            }
            fServer.SendMessage(new ServerMessage<T>(LinkedTo.LinkedActor, ServerRequest.Request, aMessage.Data));
        }
    }
}
