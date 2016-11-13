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
        private IListenerService fListener;
        public HostRelayActor()
        {
            Become(new Behavior<String>(t => { return "Listen".Equals(t); }, DoListen));
        }


        private void DoListen(Object aMsg)
        {
            try
            {
                if (fListener == null)
                {
                    fListener = ActorServer.GetInstance().ListenerService;
                }
                IContextComm context = fListener.GetCommunicationContext();
                RemoteReceiverActor.Cast(context);
                SendMessage("Listen");
            }
            catch(Exception e)
            {
                Debug.WriteLine("Can't listen " + e);
                Become(new NullBehavior());
            }
        }

                //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
                if (disposing)
                {
                    // Free other state (managed objects).
                    // fEvent.Dispose();
                }
                if (fListener != null)
                    fListener.Close();
                // Free your own state (unmanaged objects).
                // Set large fields to null.
        }

        // Use C# destructor syntax for finalization code.
        ~HostRelayActor()
        {
            // Simply call Dispose(false).

            Dispose(false);
        }

    }

}
