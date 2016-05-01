using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.IO;
using System.Diagnostics;

namespace Actor.Server
{

    class RemoteReceiverActor : BaseActor
    {

        ISerializeService fSerializeService;

        public static void Cast(IContextComm contextComm)
        {
            var remoteReceiver = new RemoteReceiverActor();
            Stream streamMessage = contextComm.ReceiveStream();
            remoteReceiver.SendMessage(contextComm,streamMessage);
        }

        public RemoteReceiverActor()
        {
            fSerializeService = ActorServer.GetInstance().SerializeService;
            Become(new Behavior<IContextComm,Stream>((i,t) => { return true; }, DoContext));
        }

        private void DoContext(IContextComm contextComm, Stream streamMessage)
        {
            // get the request stream
            using (MemoryStream ms = new MemoryStream())
            {
                streamMessage.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(ms);
                while (!sr.EndOfStream)
                {
                    var req = sr.ReadLine();
                    Debug.Print("receive " + req);
                }

                ms.Seek(0, SeekOrigin.Begin);

                SerialObject so = fSerializeService.DeSerialize(ms);
                // send an ack
                contextComm.Acknowledge();

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
