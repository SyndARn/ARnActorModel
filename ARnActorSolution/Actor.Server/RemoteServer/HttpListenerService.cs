using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Server;
using System.Globalization;
using System.Diagnostics;
using System.IO;

namespace Actor.Server
{

    class HttpListenerService : IListenerService
    {
        private HttpListener fListener;

        public HttpListenerService()
        {
            fListener = new HttpListener();
            StartServer();
        }

        public IContextComm GetCommunicationContext()
        {
            return new HttpContextComm(fListener.GetContext());
        }

        public void Close()
        {
            fListener.Close();
        }

        private void StartServer()
        {
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
            catch (Exception e)
            {
                Debug.WriteLine("Can't start http " + e);
            }
        }
    }
}
