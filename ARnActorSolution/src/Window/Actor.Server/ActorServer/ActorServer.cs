using System;
using Actor.Base;

namespace Actor.Server
{
    public class ActorServer : BaseActor, IDisposable
    {
        public string Name { get; private set; }
        public int Port { get; private set; }
        public ISerializeService SerializeService { get; private set; }
        public IListenerService ListenerService { get; private set; }
        public IHostService HostService { get; private set; }
        public IServerCommandService ServerCommandService { get; private set; }

        private const string MessageNullConfigManager = "ConfigManager can't be null";
        private string _fullHost = "" ;
        private HostRelayActor _actHostRelay;
        private ActorConfigManager _configManager;

        public string FullHost { get
        {
            if (string.IsNullOrEmpty(_fullHost))
            {
                    _fullHost = _configManager.Host().Host;
            }

            return _fullHost;
        } }

        private static ActorServer _serverInstance = null ;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static ActorServer GetInstance() => _serverInstance;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static void Start(ActorConfigManager configManager)
        {
            CheckArg.ActorConfigManager(configManager);
            ActorTagHelper.FullHost = configManager.Host().Host;

            _serverInstance = new ActorServer
            {
                _configManager = configManager ?? throw new ActorException(MessageNullConfigManager),
                Name = configManager.Host().Host,
                Port = configManager.Host().Port
            };

            _serverInstance.ListenerService = configManager.GetListenerService();
            _serverInstance.SerializeService = configManager.GetSerializeService();
            _serverInstance.HostService = configManager.GetHostService();
            _serverInstance.ServerCommandService = ActorConfigManager.GetServerCommandService();
            _serverInstance.DoInit(new HostRelayActor(_serverInstance.ListenerService));
        }

        private ActorServer()
        { }

        public ActorServer(string lName, int lPort)
        {
            _configManager = new ActorConfigManager();
            Name = lName;
            Port = lPort;
            SerializeService = _configManager.GetSerializeService();
            ActorTagHelper.FullHost = _configManager.Host().AbsoluteUri;
        }

        private void DoInit(HostRelayActor hostRelayActor)
            {
            DirectoryActor.GetDirectory(); // Start directory
            ActorConsole.Register(); // Start console
            // should work now
            SendByName.Send("Actor Server Start", "Console");
            if (ServerCommandService == null)
            {
                ServerCommandService = new ServerCommandService();
                ServerCommandService.RegisterCommand(new DiscoServerCommand());
                ServerCommandService.RegisterCommand(new StatServerCommand());
            }
            Become(ServerCommandService);
            if (hostRelayActor == null)
            {
                return;
            }

            ListenerService = _configManager.GetListenerService();
            ShardDirectoryActor.AttachShardDirectoryActor(this);
            _actHostRelay = hostRelayActor;
            _actHostRelay.SendMessage(HostRelayActor.ListenOrder);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
                {
            if (!disposing)
            {
                return;
            }

            if (_actHostRelay != null)
            {
                _actHostRelay.Dispose();
                _actHostRelay = null;
            }

            if (_configManager == null)
            {
                return;
            }

            _configManager.Dispose();
            _configManager = null;
        }

        ~ActorServer() => Dispose(false);
    }
}
