using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    sealed class actTagBinder // : SerializationBinder
    {
        //public override Type BindToType(string assemblyName, string typeName)
        //{
        //    Type typeToDeserialize = null;
        //    if ("
        //}
    }
    /*
     * sealed class Version1ToVersion2DeserializationBinder : SerializationBinder 
{
    public override Type BindToType(string assemblyName, string typeName) 
    {
        Type typeToDeserialize = null;

        // For each assemblyName/typeName that you want to deserialize to 
        // a different type, set typeToDeserialize to the desired type.
        String assemVer1 = Assembly.GetExecutingAssembly().FullName;
        String typeVer1 = "Version1Type";

        if (assemblyName == assemVer1 && typeName == typeVer1) 
        {
            // To use a type from a different assembly version,  
            // change the version number. 
            // To do this, uncomment the following line of code. 
            // assemblyName = assemblyName.Replace("1.0.0.0", "2.0.0.0");

            // To use a different type from the same assembly,  
            // change the type name.
            typeName = "Version2Type";
        }

        // The following line of code returns the type.
        typeToDeserialize = Type.GetType(String.Format("{0}, {1}", 
            typeName, assemblyName));

        return typeToDeserialize;
    }
}
     */
}
