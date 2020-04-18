using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Permissions;
using System.Globalization;
using Actor.Base;

namespace Actor.Server
{
    // http listener ...
    public class HostRelayActor : BaseActor, IDisposable
    {
        private IListenerService _listener;

        public HostRelayActor() => Become(new Behavior<String>(t => "Listen".Equals(t), DoListen));

        public HostRelayActor(IListenerService listenerService)
        {
            _listener = listenerService;
            Become(new Behavior<String>(t => "Listen".Equals(t), DoListen));
        }

        private void DoListen(object aMsg)
        {
            try
            {
                if (_listener == null)
                {
                    _listener = ActorServer.GetInstance().ListenerService;
                }

                IContextComm context = _listener.GetCommunicationContext();
                RemoteReceiverActor.Cast(context);
                SendMessage("Listen");
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
                _listener?.Close();
        }

        ~HostRelayActor()
        {
            // Simply call Dispose(false).

            Dispose(false);
        }
    }
}
