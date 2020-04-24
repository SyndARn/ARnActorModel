using System;
using System.Configuration;
using System.Globalization;

namespace Actor.Server
{
    public class ActorConfigManager : IDisposable
    {
        public static ActorConfigManager CastForTest()
        {
            ConfigurationManager.AppSettings["ListenerService"] = "MemoryListenerService";
            ConfigurationManager.AppSettings["SerializeService"] = "NetDataContractSerializeService";
            return new ActorConfigManager();
        }

        private Uri _host;
        private IHostService _hostService;

        public IHostService GetHostService()
        {
            if (_hostService == null)
            {
                string r = ConfigurationManager.AppSettings["HostService"];
                if (string.IsNullOrEmpty(r))
                {
                    _hostService = new HostService();
                }
                else
                {
                    switch (r)
                    {
                        case "HostService":
                            {
                                _hostService = new HostService();
                                break;
                            }
                        default:
                            {
                                _hostService = new HostService();
                                break;
                            }
                    }
                }
            }

            return _hostService;
        }

        public Uri Host()
        {
            if (_host == null)
            {
                string hostName = ConfigurationManager.AppSettings["HostName"];
                string hostPort = ConfigurationManager.AppSettings["HostPort"];
                if (string.IsNullOrEmpty(hostName))
                {
                    hostName = "localhost";
                }

                if (string.IsNullOrEmpty(hostPort))
                {
                    hostPort = "80";
                }

                _hostService = GetHostService();
                _host = _hostService.GetHostUri(hostName, int.Parse(hostPort,CultureInfo.InvariantCulture));
            }

            return _host;
        }

        private IListenerService _listenerService;

        public IListenerService GetListenerService()
        {
            if (_listenerService == null)
            {
                string r = ConfigurationManager.AppSettings["ListenerService"];
                if (string.IsNullOrEmpty(r))
                {
                    _listenerService = new HttpListenerService();
                }
                else
                {
                    switch (r)
                    {
                        case "HttpListenerService":
                            {
                                _listenerService = new HttpListenerService();
                                break;
                            }
                        case "MemoryListenerService":
                            {
                                _listenerService = new MemoryListenerService();
                                break;
                            }
                        default:
                            {
                                _listenerService = new HttpListenerService();
                                break;
                            }
                    }
                }
            }

            return _listenerService;
        }

        public static IServerCommandService GetServerCommandService()
        {
            ServerCommandService serverCommandService = new ServerCommandService();
            serverCommandService.RegisterCommand(new DiscoServerCommand());
            serverCommandService.RegisterCommand(new StatServerCommand());
            return serverCommandService;
        }

        private ISerializeService _serializeService;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public ISerializeService GetSerializeService()
        {
            if (_serializeService == null)
            {
                string r = ConfigurationManager.AppSettings["SerializeService"];
                // parse r to get a better thing than this with reflection
                if (string.IsNullOrEmpty(r))
                {
                    _serializeService = new NetDataContractSerializeService();
                }
                else
                {
                    switch (r)
                    {
                        case "NetDataContractSerializeService":
                            {
                                _serializeService = new NetDataContractSerializeService();
                                break;
                            }
                        default:
                            {
                                _serializeService = new NetDataContractSerializeService();
                                break;
                            }
                    }
                }
            }

            return _serializeService;
        }

        #region IDisposable Support
        private bool _disposedValue = false; // Pour détecter les appels redondants

        protected virtual void Dispose(bool disposing)
            {
            if (_disposedValue)
            {
                return;
            }

            if (disposing)
            {
                // TODO: supprimer l'état managé (objets managés).
                if (_listenerService != null)
                {
                    ((IDisposable)_listenerService).Dispose();
                    _listenerService = null;
                }
            }

            // TODO: libérer les ressources non managées (objets non managés) et remplacer un finaliseur ci-dessous.
            // TODO: définir les champs de grande taille avec la valeur Null.

            _disposedValue = true;
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
