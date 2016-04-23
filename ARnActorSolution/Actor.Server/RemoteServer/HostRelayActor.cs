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
        private HttpListener fListener;
        public HostRelayActor()
        {
            fListener = new HttpListener();
            var localhost = Dns.GetHostName();
            var servername = ActorServer.GetInstance().Name;
            var prefix = "http://";
            var suffix = ":" + ActorServer.GetInstance().Port.ToString(CultureInfo.InvariantCulture);
            fListener.Prefixes.Add(prefix + "localhost" + suffix + "/" + servername + "/");
            fListener.Prefixes.Add(prefix + localhost + suffix + "/" + servername + "/");
            fListener.Prefixes.Add(prefix + "127.0.0.1" + suffix + "/" + servername + "/");
            try
            {
                fListener.Start();
            }
            catch(Exception e)
            {
                Debug.WriteLine("Can't start http "+e );
            }
            Become(new Behavior<String>(t => { return "Listen".Equals(t); }, DoListen));
            SendMessage("Listen");
        }

        private void DoListen(Object aMsg)
        {
            try
            {
                HttpListenerContext Context = fListener.GetContext();
                new RemoteReceiverActor(Context);
                SendMessage("Listen");
            }
            catch(Exception e)
            {
                Debug.WriteLine("Can't listen " + e);
                Become(null);
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
