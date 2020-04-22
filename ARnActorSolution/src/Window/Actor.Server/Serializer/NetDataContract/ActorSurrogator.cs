using System;
using System.Runtime.Serialization;
using Actor.Base;

namespace Actor.Server
{
    public class ActorSurrogator : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info), "SerializationInfo was null");
            }

            var act = (IActor)obj;
            HostDirectoryActor.Register(act);
            // continue
            info.SetType(typeof(RemoteSenderActor));
            ActorTag remoteTag = act.Tag;
            info.AddValue("RemoteTag", remoteTag, typeof(ActorTag));
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            // Reset the property value using the GetValue method.

            // force misc init
            if (obj is RemoteSenderActor remoteActor)
            {
                if (info == null)
                {
                    throw new ActorException("Receiving null SerializationInfo");
                }

                BaseActor.CompleteInitialize(remoteActor);
                RemoteSenderActor.CompleteInitialize(remoteActor);
                var getTag = (ActorTag)info.GetValue("RemoteTag", typeof(ActorTag));
                typeof(RemoteSenderActor).GetField("fRemoteTag").SetValue(obj, getTag);
            }

            return null; // ms bug here
        }
    }
}
