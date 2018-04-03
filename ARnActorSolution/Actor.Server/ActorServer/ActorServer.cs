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
        private string fFullHost = "" ;
        private HostRelayActor fActHostRelay;
        private ConfigManager fConfigManager;
        public string FullHost { get 
        {
            if (string.IsNullOrEmpty(fFullHost))
            {
                    fFullHost = fConfigManager.Host().Host;
            }
            return fFullHost;
        } }

        private static ActorServer fServerInstance = null ;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static ActorServer GetInstance()
        {
            return fServerInstance ;
        }

        public static void Start(string lName, int lPort, HostRelayActor hostRelayActor)
        {
            fServerInstance = new ActorServer(lName,lPort) ;
            fServerInstance.DoInit(hostRelayActor);
        }

        public ActorServer(string lName, int lPort)
        {
            fConfigManager = new ConfigManager();
            Name = lName;
            Port = lPort;
            SerializeService = fConfigManager.GetSerializeService();
            ActorTagHelper.FullHost = fConfigManager.Host().Host;
        }

        private void DoInit(HostRelayActor hostRelayActor) 
        {
            DirectoryActor.GetDirectory(); // Start directory
            ActorConsole.Register(); // Start console
            // should work now
            SendByName<string>.Send("Actor Server Start", "Console");
            Become(new NullBehavior());
            if (hostRelayActor != null)
            {
                ListenerService = fConfigManager.GetListenerService();
                new ShardDirectoryActor(); // start shard directory
                fActHostRelay = hostRelayActor;
                fActHostRelay.SendMessage("Listen");
            }
            // new actTcpServer();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (fActHostRelay != null)
                {
                    fActHostRelay.Dispose();
                    fActHostRelay = null;
                }
                if (fConfigManager != null)
                {
                    fConfigManager.Dispose();
                    fConfigManager = null;
                }
            }
        }

        ~ActorServer()
        {
            Dispose(false);
        }
    }
} 
