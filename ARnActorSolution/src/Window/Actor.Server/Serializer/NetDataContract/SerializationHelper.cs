using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Server
{
    public class ActorBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type outtype = null;
            Type typefound = Type.GetType(string.Format(CultureInfo.InvariantCulture,"{0}, {1}", typeName, assemblyName));
            if (typefound != null)
            {
                if (typefound.IsSubclassOf(typeof(BaseActor)))
                {
                    outtype = typeof(RemoteSenderActor);
                }
            }
            return outtype;
        }
    }

    public class ActorSurrogatorSelector : SurrogateSelector
    {
        private const string MessageNullInGetSurrogate = "Null type found in GetSurrogate";

        public ActorSurrogatorSelector()
            : base()
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public override ISerializationSurrogate GetSurrogate(
            Type type,
            StreamingContext context,
            out ISurrogateSelector selector
            )
        {
            if (type == null)
            {
                throw new ActorException(MessageNullInGetSurrogate);
            }
            if (type.IsSubclassOf(typeof(BaseActor)))
            {
                Debug.WriteLine("push actor {0} to host directory", type);
                selector = this;
                return new ActorSurrogator();
            }
            else
            {
                return base.GetSurrogate(type, context, out selector);
            }
        }
    }
}
