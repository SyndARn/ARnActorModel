using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Server
{
    public class ActorServer : BaseActor, IDisposable
    {
        public string Name { get; private set; }
        public int Port { get; private set; }
        private string fFullHost = "" ;
        private actHostRelay fActHostRelay;
        public string FullHost { get 
        {
            if (string.IsNullOrEmpty(fFullHost))
            {
                fFullHost = Fullhost();
            }
            return fFullHost;
        } }

        private string Fullhost()
        {
            var localhost = Dns.GetHostName();
            var prefix = "http://";
            var suffix = ":" + Port.ToString();
            var fullhost = prefix + localhost + suffix + "/" + Name + "/";
            return fullhost;
        }

        private static ActorServer fServerInstance = null ;

        public static ActorServer GetInstance()
        {
            return fServerInstance ;
        }

        public static void Start(string lName, int lPort, bool withRelay = true)
        {
            fServerInstance = new ActorServer(lName,lPort) ;
            fServerInstance.DoInit(withRelay);
        }

        public ActorServer(string lName, int lPort)
        {
            Name = lName;
            Port = lPort;
            actTagHelper.SetFullHost(Fullhost());
        }

        private void DoInit(bool withRelay) 
        {
            actDirectory.GetDirectory(); // Start directory
            new ActorConsole(); // Start console
            // should work now
            SendByName<string>.Send("Actor Server Start", "Console");
            Become(null);
            if (withRelay)
            {
                new actShardDirectory(); // start shard directory
                fActHostRelay = new actHostRelay();
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
