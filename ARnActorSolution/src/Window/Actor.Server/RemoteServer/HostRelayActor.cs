using System;
using System.Diagnostics;
using Actor.Base;

namespace Actor.Server
{
    // http listener ...
    public class HostRelayActor : BaseActor, IDisposable
    {
        private IListenerService fListener;
        public const string ListenOrder = "Listen";

        public HostRelayActor() => Become(new Behavior<string>(t => ListenOrder.Equals(t,StringComparison.InvariantCulture), DoListen));

        public HostRelayActor(IListenerService listenerService)
        {
            fListener = listenerService;
            Become(new Behavior<String>(t => ListenOrder.Equals(t, StringComparison.InvariantCulture), DoListen));
        }

        private void DoListen(object aMsg)
        {
            try
            {
                if (fListener == null)
                {
                    fListener = ActorServer.GetInstance().ListenerService;
                }
                IContextComm context = fListener.GetCommunicationContext();
                RemoteReceiverActor.Cast(context);
                SendMessage(ListenOrder);
            }
            catch(Exception e)
            {
                Debug.WriteLine("Can't listen " + e);
                Become(new NullBehavior());
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
                if (disposing)
                {
                }
                fListener?.Close();
        }

        ~HostRelayActor()
        {
            // Simply call Dispose(false).

            Dispose(false);
        }
    }
}
