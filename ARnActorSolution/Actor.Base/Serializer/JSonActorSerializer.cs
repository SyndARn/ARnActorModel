using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
        static class JSonActorSerializer
        {
            public static SerialObject DeSerialize(Stream inputStream)
            {
                // TODO
                //inputStream.Seek(0, SeekOrigin.Begin);
                //DataContractJsonSerializer dcs = new DataContractJsonSerializer(typeof(SerialObject));
                //dcs.SurrogateSelector = new ActorSurrogatorSelector();
                //dcs.Binder = new ActorBinder();
                // return (SerialObject)dcs.ReadObject(inputStream);
                return null;
            }

            public static void Serialize(SerialObject so, Stream outputStream)
            {
                // TODO
                //DataContractJsonSerializer dcs = new DataContractJsonSerializer(typeof(SerialObject));
                //dcs.SurrogateSelector = new ActorSurrogatorSelector();
                //dcs.Binder = new ActorBinder();
                //dcs.Serialize(outputStream, so);
            }
        }
}
