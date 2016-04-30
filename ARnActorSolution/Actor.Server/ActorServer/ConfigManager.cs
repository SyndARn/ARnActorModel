using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.Globalization;

namespace Actor.Server
{
    public interface IHostService
    {
        Uri GetHostUri(string Name, int Port);
    }

    public class HostService : IHostService
    {
        public Uri GetHostUri(string Name, int Port)
        {
            var localhost = Dns.GetHostName();
            var prefix = "http://";
            var suffix = ":" + Port.ToString(CultureInfo.InvariantCulture);
            var fullhost = prefix + localhost + suffix + "/" + Name + "/";

            return new Uri(fullhost);
        }
    }

    public class ConfigManager
    {

        private Uri fHost;

        public Uri Host()
        {
            if (fHost == null)
            {
                string r = ConfigurationManager.AppSettings["HostService"];
                string name = ConfigurationManager.AppSettings["HostName"];
                string port = ConfigurationManager.AppSettings["HostPort"];
                if (string.IsNullOrEmpty(name))
                {
                    name = "ARnActorServer";
                }
                if (string.IsNullOrEmpty(port))
                {
                    port = "80";
                }
                // parse r to get a better thing than this with reflection
                if (string.IsNullOrEmpty(r))
                {
                    // var serv = new HostService();
                }
                var serv = new HostService();
                fHost =  serv.GetHostUri(name, int.Parse(port));
            }
            return fHost;
        }

        private IListenerService fListenerService;
        public IListenerService GetListenerService()
        {
            if (fListenerService == null)
            {
                fListenerService = new HttpListenerService();
            }
            return fListenerService;
            //fListener = new HttpListener();
            //var localhost = Dns.GetHostName();
            //var servername = ActorServer.GetInstance().Name;
            //var prefix = "http://";
            //var suffix = ":" + ActorServer.GetInstance().Port.ToString(CultureInfo.InvariantCulture);
            //fListener.Prefixes.Add(prefix + "localhost" + suffix + "/" + servername + "/");
            //fListener.Prefixes.Add(prefix + localhost + suffix + "/" + servername + "/");
            //fListener.Prefixes.Add(prefix + "127.0.0.1" + suffix + "/" + servername + "/");
            //try
            //{
            //    fListener.Start();
            //}
            //catch (Exception e)
            //{
            //    Debug.WriteLine("Can't start http " + e);
            //}
        }

        private ISerializeService fSerializeService;

        public ISerializeService GetSerializeService()
        {
            if (fSerializeService == null)
            {
                string r = ConfigurationManager.AppSettings["SerializeService"];
                // parse r to get a better thing than this with reflection
                if (string.IsNullOrEmpty(r))
                {
                    fSerializeService = new NetDataContractSerializeService();
                }
                else
                {
                    switch (r)
                    {
                        case "NetDataContractSerializeService":
                            {
                                fSerializeService = new NetDataContractSerializeService();
                                break;
                            }
                        default:
                            {
                                fSerializeService = new NetDataContractSerializeService();
                                break;
                            }
                    }
                }
                
            }
            return fSerializeService;
        }
    }
}
