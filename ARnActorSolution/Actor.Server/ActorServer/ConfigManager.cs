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
        Uri GetHostUri(string name, int port);
    }

    public class HostService : IHostService
    {
        public Uri GetHostUri(string name, int port)
        {
            var prefix = "http://";
            var suffix = ":" + port.ToString(CultureInfo.InvariantCulture);
            var fullhost = prefix + name + suffix + "/" ;

            return new Uri(fullhost);
        }
    }

    public class ConfigManager : IDisposable
    {

        public static ConfigManager CastForTest()
        {
            ConfigurationManager.AppSettings["ListenerService"] = "MemoryListenerService";
            ConfigurationManager.AppSettings["SerializeService"] = "NetDataContractSerializeService";
            return new ConfigManager();
        }

        private Uri fHost;

        public Uri Host()
        {
            if (fHost == null)
            {
                string hostService = ConfigurationManager.AppSettings["HostService"];
                string hostName = ConfigurationManager.AppSettings["HostName"];
                string hostPort = ConfigurationManager.AppSettings["HostPort"];
                if (string.IsNullOrEmpty(hostName))
                {
                    hostName = "ARnActorServer";
                }
                if (string.IsNullOrEmpty(hostPort))
                {
                    hostPort = "80";
                }
                // parse r to get a better thing than this with reflection
                if (string.IsNullOrEmpty(hostService))
                {
                    // var serv = new HostService();
                }
                var serv = new HostService();
                fHost =  serv.GetHostUri(hostName, int.Parse(hostPort,CultureInfo.InvariantCulture));
            }
            return fHost;
        }

        private IListenerService fListenerService;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Supprimer les objets avant la mise hors de portée")]
        public IListenerService GetListenerService()
        {
            if (fListenerService == null)
            {
                string r = ConfigurationManager.AppSettings["ListenerService"];
                if (string.IsNullOrEmpty(r))
                {
                    fListenerService = new HttpListenerService();
                }
                else
                {
                    switch (r)
                    {
                        case "HttpListenerService":
                            {
                                fListenerService = new HttpListenerService();
                                break;
                            }
                        case "MemoryListenerService":
                            {
                                fListenerService = new MemoryListenerService();
                                break;
                            }
                        default:
                            {
                                fListenerService = new HttpListenerService();
                                break;
                            }
                    }
                }
            }
            return fListenerService;

        }

        private ISerializeService fSerializeService;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
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

        #region IDisposable Support
        private bool disposedValue = false; // Pour détecter les appels redondants

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: supprimer l'état managé (objets managés).
                    if (fListenerService != null)
                    {
                        ((IDisposable)fListenerService).Dispose();
                        fListenerService = null;
                    }
                }

                // TODO: libérer les ressources non managées (objets non managés) et remplacer un finaliseur ci-dessous.
                // TODO: définir les champs de grande taille avec la valeur Null.

                disposedValue = true;
            }
        }

        // TODO: remplacer un finaliseur seulement si la fonction Dispose(bool disposing) ci-dessus a du code pour libérer les ressources non managées.
        // ~ConfigManager() {
        //   // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
        //   Dispose(false);
        // }

        // Ce code est ajouté pour implémenter correctement le modèle supprimable.
        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(true);
            // TODO: supprimer les marques de commentaire pour la ligne suivante si le finaliseur est remplacé ci-dessus.
             GC.SuppressFinalize(this);
        }
        #endregion
    }
}
