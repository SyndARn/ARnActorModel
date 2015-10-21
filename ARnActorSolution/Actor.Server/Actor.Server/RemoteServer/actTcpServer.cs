using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization;
using Actor.Server;

namespace Actor.Base
{
    public class actTcpServer : actActor
    {
        TcpListener fTcpListener;
        IPEndPoint fEndPoint;

        public actTcpServer()
        {
            fEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
            fTcpListener = new TcpListener(fEndPoint);
            Become(new bhvBehavior<string>(DoStartListen));
            fTcpListener.Start();
            SendMessage("Start Listen");
        }

        private void DoStartListen(string msg)
        {
            Task<TcpClient> client = fTcpListener.AcceptTcpClientAsync();
            IActor entryConnection = new actEntryConnection();
            entryConnection.SendMessage(client.Result);
            SendMessage("Continue Listen");
        }
    }

    public class actEntryConnection : actAction<TcpClient>
    {
        private void DoListen(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            MemoryStream memStream = new MemoryStream();
            stream.CopyTo(memStream);
            StreamReader sr = new StreamReader(memStream);
            while (!sr.EndOfStream)
            {
                var req = sr.ReadLine();
                Debug.Print("receive " + req);
            }
            memStream.Seek(0, SeekOrigin.Begin);
            NetDataContractSerializer dcs = new NetDataContractSerializer();
            dcs.SurrogateSelector = new ActorSurrogatorSelector();
            dcs.Binder = new ActorBinder();
            Object obj = dcs.ReadObject(memStream);
            SerialObject so = (SerialObject)obj;

            // no answer expected
            client.Close();

            // find hosted actor directory
            // forward msg to hostedactordirectory
            SendMessage(so);
        }

        private void DoProcessMessage(SerialObject aSerial)
        {
            // disco ?
            if ((aSerial.Data != null) && (aSerial.Data.GetType().Equals(typeof(DiscoCommand))))
            {
                // ask directory entries for server
                //actHostDirectory.Register(this);
                actDirectory.GetDirectory().Disco(((DiscoCommand)aSerial.Data).Sender);
            }
            else
            {
                // or send to host directory
                actHostDirectory.GetInstance().SendMessage(aSerial);
            }
        }
    }
}
