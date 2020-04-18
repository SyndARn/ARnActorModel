using System.Collections.Generic;
using Actor.Base;

namespace Actor.Server
{
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
}
