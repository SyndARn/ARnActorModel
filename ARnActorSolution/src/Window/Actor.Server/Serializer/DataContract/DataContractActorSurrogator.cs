using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

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
            if (obj is IActor act)
            {
                HostDirectoryActor.Register(act);
                ActorTag remoteTag = act.Tag;
                return act;
            }
            return obj;
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

            return obj;
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
}
