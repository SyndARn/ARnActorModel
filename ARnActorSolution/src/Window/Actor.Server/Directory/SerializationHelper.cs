using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{

    class ActorBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type outtype = null;
            Type typefound = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
            if (typefound != null)
            {
                if (typefound.IsSubclassOf(typeof(actActor)))
                {
                    outtype = typeof(actRemoteActor);
                }
            }
            return outtype;
        }
    }

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


            // force misc init
            if (obj is actRemoteActor)
            {
                actActor.CompleteInitialize((actActor)obj);
                actRemoteActor.CompleteInitialize((actRemoteActor)obj);
                actTag getTag = (actTag)info.GetValue("RemoteTag", typeof(actTag));
                typeof(actRemoteActor).GetField("fRemoteTag").SetValue(obj, getTag);
            }


            return null; // ms bug here
        }
    }

    class ActorSurrogatorSelector : SurrogateSelector
    {
        public ActorSurrogatorSelector()
            : base()
        {
        }

        public override ISerializationSurrogate GetSurrogate(
            Type type,
            StreamingContext context,
            out ISurrogateSelector selector
            )
        {
            if (type.IsSubclassOf(typeof(actActor)))
            {
                Debug.WriteLine("push actor {0} to host directory", type);
                selector = this;
                return new ActorSurrogator();
            }
            else
                return base.GetSurrogate(type, context, out selector);
        }
    }
}
