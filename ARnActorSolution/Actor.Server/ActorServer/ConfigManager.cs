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
            var localhost = Dns.GetHostName();
            var prefix = "http://";
            var suffix = ":" + port.ToString(CultureInfo.InvariantCulture);
            var fullhost = prefix + localhost + suffix + "/" + name + "/";

            return new Uri(fullhost);
        }
    }

    public class ConfigManager : IDisposable
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
                fHost =  serv.GetHostUri(name, int.Parse(port,CultureInfo.InvariantCulture));
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
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
