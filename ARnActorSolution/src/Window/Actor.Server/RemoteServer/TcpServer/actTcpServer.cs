/*****************************************************************************
		               ARnActor Actor Model Library .Net
     
	 Copyright (C) {2015}  {ARn/SyndARn} 
 
 
     This program is free software; you can redistribute it and/or modify 
     it under the terms of the GNU General Public License as published by 
     the Free Software Foundation; either version 2 of the License, or 
     (at your option) any later version. 
 
 
     This program is distributed in the hope that it will be useful, 
     but WITHOUT ANY WARRANTY; without even the implied warranty of 
     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
     GNU General Public License for more details. 
 
 
     You should have received a copy of the GNU General Public License along 
     with this program; if not, write to the Free Software Foundation, Inc., 
     51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA. 
*****************************************************************************/
using System;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization;
using Actor.Base;

namespace Actor.Server
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "act")]
    public class ActorTcpServer : BaseActor
    {
        TcpListener fTcpListener;
        IPEndPoint fEndPoint;

        public ActorTcpServer()
        {
            fEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
            fTcpListener = new TcpListener(fEndPoint);
            Become(new Behavior<string>(DoStartListen));
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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "act")]
    public class actEntryConnection : BaseActor
    {
        public actEntryConnection() : base()
        {
            Become(new Behavior<TcpClient>(DoListen));
        }

        private void DoListen(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            using (MemoryStream memStream = new MemoryStream())
            {
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
                Become(new Behavior<SerialObject>(t => { return true; }, DoProcessMessage));
                SendMessage(so);
            }
        }

        private void DoProcessMessage(SerialObject aSerial)
        {
            // disco ?
            if ((aSerial.Data != null) && (aSerial.Data.GetType().Equals(typeof(DiscoCommand))))
            {
                // ask directory entries for server
                //actHostDirectory.Register(this);
                DirectoryActor.GetDirectory().Disco(((DiscoCommand)aSerial.Data).Sender);
            }
            else
            {
                // or send to host directory
                HostDirectoryActor.GetInstance().SendMessage(aSerial);
            }
        }
    }
}
