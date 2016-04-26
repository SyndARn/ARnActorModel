using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace Actor.Server
{
    class RemoteReceiverActor : BaseActor
    {

        ISerializeService fSerializeService;

        public static void Cast(HttpListenerContext aContext)
        {
            var remoteReceiver = new RemoteReceiverActor();
            remoteReceiver.SendMessage(aContext);
        }

        public RemoteReceiverActor()
        {
            fSerializeService = ActorServer.GetInstance().SerializeService;
            Become(new Behavior<HttpListenerContext>(t => { return true; }, DoContext));            
        }

        private void DoContext(HttpListenerContext aContext)
        {
            // get the request stream
            Stream str = aContext.Request.InputStream;
            using (MemoryStream ms = new MemoryStream())
            {
                str.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(ms);
                while (!sr.EndOfStream)
                {
                    var req = sr.ReadLine();
                    Debug.Print("receive " + req);
                }

                ms.Seek(0, SeekOrigin.Begin);

                // SerialObject so = NetDataActorSerializer.DeSerialize(ms);
                SerialObject so = fSerializeService.DeSerialize(ms);
                // prepare an answer
                HttpListenerResponse Response = aContext.Response;

                // write something to response ...
                Response.Close();

                // find hosted actor directory
                // forward msg to hostedactordirectory
                Become(new Behavior<SerialObject>(t => { return true; }, ProcessMessage));
                SendMessage(so);
            }

        }

        private void ProcessMessage(SerialObject aSerial)
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
