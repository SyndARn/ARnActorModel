using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Server
{
    class ActorSurrogator : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            IActor act = (IActor)obj;
            HostDirectoryActor.Register(act);
            // continue
            info.SetType(typeof(RemoteActor));
            ActorTag remoteTag = act.Tag;
            info.AddValue("RemoteTag", remoteTag, typeof(ActorTag));
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            // Reset the property value using the GetValue method.

            RemoteActor remoteActor = obj as RemoteActor;
            // force misc init
            if (remoteActor != null)
            {
                if (info == null)
                {
                    throw new ActorException("Receiving null SerializationInfo");
                }
                BaseActor.CompleteInitialize(remoteActor);
                RemoteActor.CompleteInitialize(remoteActor);
                ActorTag getTag = (ActorTag)info.GetValue("RemoteTag", typeof(ActorTag));
                typeof(RemoteActor).GetField("fRemoteTag").SetValue(obj, getTag);
            }


            return null; // ms bug here
        }
    }
}
