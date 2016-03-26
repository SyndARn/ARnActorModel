using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    class ActorSurrogator : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            IActor act = (IActor)obj;
            actHostDirectory.Register(act);
            // continue
            info.SetType(typeof(actRemoteActor));
            actTag remoteTag = act.Tag;
            info.AddValue("RemoteTag", remoteTag, typeof(actTag));
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            // Reset the property value using the GetValue method.

            actRemoteActor remoteActor = obj as actRemoteActor;
            // force misc init
            if (remoteActor != null)
            {
                BaseActor.CompleteInitialize(remoteActor);
                actRemoteActor.CompleteInitialize(remoteActor);
                actTag getTag = (actTag)info.GetValue("RemoteTag", typeof(actTag));
                typeof(actRemoteActor).GetField("fRemoteTag").SetValue(obj, getTag);
            }


            return null; // ms bug here
        }
    }
}
