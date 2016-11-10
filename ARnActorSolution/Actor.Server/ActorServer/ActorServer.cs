using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Globalization;

[assembly: CLSCompliant(true)]
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
        public string FullHost { get 
        {
            if (string.IsNullOrEmpty(fFullHost))
            {
                    fFullHost = new ConfigManager().Host().Host;
            }
            return fFullHost;
        } }

        private static ActorServer fServerInstance = null ;

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
            Name = lName;
            Port = lPort;
            SerializeService = new ConfigManager().GetSerializeService();

            ActorTagHelper.FullHost = new ConfigManager().Host().Host;
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
                ListenerService = new ConfigManager().GetListenerService();
                new ShardDirectoryActor(); // start shard directory
                fActHostRelay = hostRelayActor;
                fActHostRelay.SendMessage("Listen");
            }
            // new actTcpServer();
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
                fActHostRelay.Dispose();
            }
            // Free your own state (unmanaged objects).
            // Set large fields to null.
        }

        // Use C# destructor syntax for finalization code.
        ~ActorServer()
        {
            // Simply call Dispose(false).

            Dispose(false);
        }
    }
} 
