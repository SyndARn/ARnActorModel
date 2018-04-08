using Actor.Base;
using System.IO;
using System.Diagnostics;

namespace Actor.Server
{
    internal class RemoteReceiverActor : BaseActor
    {
        private readonly ISerializeService fSerializeService;

        public static void Cast(IContextComm contextComm)
        {
            var remoteReceiver = new RemoteReceiverActor();
            Stream streamMessage = contextComm.ReceiveStream();
            remoteReceiver.SendMessage(contextComm,streamMessage);
        }

        public RemoteReceiverActor()
        {
            fSerializeService = ActorServer.GetInstance().SerializeService;
            Become(new Behavior<IContextComm,Stream>((i, t) => true, DoContext));
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

                object so = fSerializeService.Deserialize(ms);
                // send an ack
                contextComm.Acknowledge();

                // find hosted actor directory
                // forward msg to hostedactordirectory
                Become(new Behavior<object>(t => true, ProcessMessage));
                SendMessage(so);
            }
        }

        private void ProcessMessage(object tobeSerial)
        {
            SerialObject aSerial = tobeSerial as SerialObject;
                if (tobeSerial is DataContractObject)
                {
                aSerial = new SerialObject(((DataContractObject)tobeSerial).Data, ((DataContractObject)tobeSerial).Tag);
                }

            // disco ?
            if (aSerial.Data?.GetType().Equals(typeof(DiscoCommand)) == true)
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
