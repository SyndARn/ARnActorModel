using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{

    public class actRemoteActor : actActor
    {

        public actTag fRemoteTag;

        public static void CompleteInitialize(actRemoteActor anActor)
        {
            anActor.Become(new bhvBehavior<Object>(anActor.DoRouting));
        }

        public actRemoteActor(actTag aTag)
            : base()
        {
            fRemoteTag = aTag;
            Become(new bhvBehavior<Object>(DoRouting));
        }

        private void DoRouting(Object aMsg)
        {
            SendRemoteMessage(aMsg);
        }

        private void SendRemoteMessage(Object aMsg)
        {
            // send message with http
            SerialObject so = new SerialObject();
            so.Data = aMsg;
            so.Tag = fRemoteTag;

            NetDataContractSerializer dcs = new NetDataContractSerializer();
            dcs.SurrogateSelector = new ActorSurrogatorSelector();
            dcs.Binder = new ActorBinder();
            MemoryStream ms = new MemoryStream();
            dcs.Serialize(ms, so);
            ms.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(ms);
            while (!sr.EndOfStream)
                Debug.Print(sr.ReadLine());
            ms.Seek(0, SeekOrigin.Begin);
            // No response expected
            using (var client = new HttpClient())
            {
                using (var hc = new StreamContent(ms))
                {
                    Uri uri = new Uri(so.Tag.Uri);
                    client.PostAsync(uri, hc).Wait();
                }
            }
        }
    }

}
