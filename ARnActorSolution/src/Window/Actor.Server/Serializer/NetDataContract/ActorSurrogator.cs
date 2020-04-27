using System;
using System.Runtime.Serialization;
using Actor.Base;

namespace Actor.Server
{
    public class ActorSurrogator : ISerializationSurrogate
    {
        private const string MessageReceivingNullSerializationInfo = "Receiving null SerializationInfo";
        private const string MessageNullSerializationInfo = "SerializationInfo was null";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {

                throw new ArgumentNullException(nameof(info), MessageNullSerializationInfo);
            }

            IActor act = (IActor)obj;
            HostDirectoryActor.Register(act);
            // continue
            info.SetType(typeof(RemoteSenderActor));
            ActorTag remoteTag = act.Tag;
            info.AddValue("RemoteTag", remoteTag, typeof(ActorTag));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            // Reset the property value using the GetValue method.

            // force misc init
            if (obj is RemoteSenderActor remoteActor)
            {
                if (info == null)
                {
                    throw new ActorException(MessageReceivingNullSerializationInfo);
                }

                BaseActor.CompleteInitialize(remoteActor);
                RemoteSenderActor.CompleteInitialize(remoteActor);
                ActorTag getTag = (ActorTag)info.GetValue("RemoteTag", typeof(ActorTag));
                typeof(RemoteSenderActor).GetField("fRemoteTag").SetValue(obj, getTag);
            }

            return null; // ms bug here
        }
    }
}
