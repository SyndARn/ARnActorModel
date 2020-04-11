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
        private string _fullHost = "" ;
        private HostRelayActor _actHostRelay;
        private ConfigManager _configManager;

        public string FullHost { get
        {
            if (string.IsNullOrEmpty(_fullHost))
            {
                    _fullHost = _configManager.Host().Host;
            }
            return _fullHost;
        } }

        private static ActorServer fServerInstance = null ;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static ActorServer GetInstance() => fServerInstance;

        public static void Start(ConfigManager configManager)
        {
            fServerInstance = new ActorServer
            {
                _configManager = configManager ?? throw new ActorException("ConfigManager can't be null"),
                Name = configManager.Host().Host,
                Port = configManager.Host().Port,
                ListenerService = configManager.GetListenerService(),
                SerializeService = configManager.GetSerializeService(),
                HostService = configManager.GetHostService()
            };
            ActorTagHelper.FullHost = configManager.Host().Host;
            fServerInstance.DoInit(new HostRelayActor(fServerInstance.ListenerService));
        }

        public static void Start(string lName, int lPort, HostRelayActor hostRelayActor)
        {
            fServerInstance = new ActorServer(lName,lPort) ;
            fServerInstance.DoInit(hostRelayActor);
        }

        private ActorServer()
        { }

        public ActorServer(string lName, int lPort)
        {
            _configManager = new ConfigManager();
            Name = lName;
            Port = lPort;
            SerializeService = _configManager.GetSerializeService();
            ActorTagHelper.FullHost = _configManager.Host().Host;
        }

        private void DoInit(HostRelayActor hostRelayActor)
            {
            DirectoryActor.GetDirectory(); // Start directory
            ActorConsole.Register(); // Start console
            // should work now
            SendByName<string>.Send("Actor Server Start", "Console");
            Become(new NullBehavior());
            if (hostRelayActor == null)
            {
                return;
            }

            ListenerService = _configManager.GetListenerService();
            new ShardDirectoryActor(this); // start shard directory
            _actHostRelay = hostRelayActor;
            _actHostRelay.SendMessage("Listen");
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
