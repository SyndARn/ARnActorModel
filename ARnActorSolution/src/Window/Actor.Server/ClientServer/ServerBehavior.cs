using System.Collections.Generic;
using Actor.Base;

namespace Actor.Server
{
    public abstract class ServerBehavior<T> : Behavior<ServerMessage<T>>
    {
        private const string MessageServerBehaviorNullMessage = "Server Behavior, null message encounters";
        private const string MessageNullMessageReceiveInServerApply = "Null message receive in ServerApply";
        private const string MessageNullClientEncountered = "Null client encountered";
        private readonly List<IActor> _actorList = new List<IActor>();

        protected ServerBehavior() : base()
        {
            Pattern = ServerPattern;
            Apply = ServerApply;
        }

        public bool ServerPattern(ServerMessage<T> aMessage) => true;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public void ServerApply(ServerMessage<T> aMessage)
        {
            if (aMessage == null)
            {
                throw new ActorException(MessageNullMessageReceiveInServerApply);
            }
            switch (aMessage.Request)
            {
                case ServerRequest.Connect: DoConnect(aMessage); break;
                case ServerRequest.Disconnect: DoDisconnect(aMessage); break;
                case ServerRequest.Request: DoRequest(aMessage); break;
                default: break;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        protected void DoConnect(ServerMessage<T> aMessage)
        {
            if (aMessage == null)
            {
                throw new ActorException(MessageServerBehaviorNullMessage);
            }
            if (!_actorList.Contains(aMessage.Client))
            {
                _actorList.Add(aMessage.Client);
            }
            aMessage.Client.SendMessage(new ServerMessage<T>(aMessage.Client, ServerRequest.Accept, default));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        protected void DoDisconnect(ServerMessage<T> aMessage)
        {
            if (aMessage == null)
            {
                throw new ActorException(MessageServerBehaviorNullMessage);
            }
            if (_actorList.Contains(aMessage.Client))
            {
                _actorList.Remove(aMessage.Client);
            }
        }

        protected abstract void DoRequest(ServerMessage<T> aMessage);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public void SendAnswer(ServerMessage<T> aMessage, T data)
        {
            if (aMessage == null)
            {
                throw new ActorException(MessageServerBehaviorNullMessage);
            }
            if (aMessage.Client == null)
            {
                throw new ActorException(MessageNullClientEncountered);
            }
            aMessage.Client.SendMessage(new ServerMessage<T>(aMessage.Client, ServerRequest.Answer, data));
        }
    }
}
