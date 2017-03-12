using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Actor.Server
{

    public class DataContractActorSurrogate : IDataContractSurrogate
    {
        public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
        {
            return null;
        }

        public object GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            return null;
        }

        public Type GetDataContractType(Type type)
        {
            if (typeof(IActor).IsAssignableFrom(type))
            {
                return typeof(IActor);
            }
            return type;
        }

        public object GetDeserializedObject(object obj, Type targetType)
        {
            IActor act = (IActor)obj;
            HostDirectoryActor.Register(act);
            // continue
            ActorTag remoteTag = act.Tag;
            return act;
        }

        public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
        {
            throw new NotImplementedException();
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            // Reset the property value using the GetValue method.

            // force misc init
            if (obj is RemoteSenderActor remoteActor)
            {
                BaseActor.CompleteInitialize(remoteActor);
                RemoteSenderActor.CompleteInitialize(remoteActor);
                // typeof(RemoteSenderActor).GetField("fRemoteTag").SetValue(obj, getTag);
            }


            return null; // ms bug here
        }

        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            //if (typeof(IActor).IsAssignableFrom(IActor))
            //{
            //    return typeof(IActor);
            //}
            //return type;
            return null;
        }

        public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
        {
            return typeDeclaration;
        }
    }

    public class ActorSurrogator : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info","SerializationInfo was null");
            }
            IActor act = (IActor)obj;
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
                ActorTag getTag = (ActorTag)info.GetValue("RemoteTag", typeof(ActorTag));
                typeof(RemoteSenderActor).GetField("fRemoteTag").SetValue(obj, getTag);
            }
            return null; // ms bug here
        }
    }
}
