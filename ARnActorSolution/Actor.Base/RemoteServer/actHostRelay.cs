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

namespace Actor.Base
{

    [Serializable]
    public class SerialObject
    {
        public Object Data { get; set; }
        public actTag Tag { get; set; }
        public SerialObject() {  }
    }

    // http listener ...
    public class actHostRelay : actActor, IDisposable
    {
        HttpListener Listener;
        public actHostRelay()
        {
            Listener = new HttpListener();
            var localhost = Dns.GetHostName();
            var servername = ActorServer.GetInstance().Name;
            var prefix = "http://";
            var suffix = ":" + ActorServer.GetInstance().Port.ToString();
            Listener.Prefixes.Add(prefix + localhost + suffix + "/" + servername + "/");
            Listener.Prefixes.Add(prefix + "127.0.0.1" + suffix + "/" + servername + "/");
            try
            {
                Listener.Start();
            }
            catch(Exception e)
            {
                Debug.WriteLine("Can't start http "+e );
            }
            Become(new bhvBehavior<String>(t => { return "Listen".Equals(t); }, DoListen));
            SendMessage("Listen");
        }

        private void DoListen(Object aMsg)
        {
            try
            {
                HttpListenerContext Context = Listener.GetContext();
                new actProcessRelayRequest(Context);
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
                if (Listener != null)
                    Listener.Close();
                // Free your own state (unmanaged objects).
                // Set large fields to null.
        }

        // Use C# destructor syntax for finalization code.
        ~actHostRelay()
        {
            // Simply call Dispose(false).

            Dispose(false);
        }

    }

    class actProcessRelayRequest : actActor
    {
        public actProcessRelayRequest(HttpListenerContext aContext)
        {
            Become(new bhvBehavior<HttpListenerContext>(t => { return true; }, DoContext));
            SendMessage(aContext);
        }

        private void DoContext(HttpListenerContext aContext)
        {
            // get the request stream
            Stream str = aContext.Request.InputStream;
            MemoryStream ms = new MemoryStream();
            str.CopyTo(ms);
            ms.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(ms);
            while (!sr.EndOfStream)
            {
                var req = sr.ReadLine();
                Debug.Print("receive " + req);
            }

            SerialObject so = NetDataActorSerializer.DeSerialize(ms);

            // prepare an answer
            HttpListenerResponse Response = aContext.Response;
            
            // write something to response ...
            Response.Close();
            
            // find hosted actor directory
            // forward msg to hostedactordirectory
            Become(new bhvBehavior<SerialObject>(t => { return true; }, ProcessMessage));
            SendMessage(so);

        }

        private void ProcessMessage(SerialObject aSerial)
        {
            // disco ?
            if ((aSerial.Data != null) && (aSerial.Data.GetType().Equals(typeof(DiscoCommand))))
            {
                // ask directory entries for server
                //actHostDirectory.Register(this);
                actDirectory.GetDirectory().Disco(((DiscoCommand)aSerial.Data).Sender); 
            }
            else
            {
                // or send to host directory
                actHostDirectory.GetInstance().SendMessage(aSerial) ; 
            }
        }
    }
}
