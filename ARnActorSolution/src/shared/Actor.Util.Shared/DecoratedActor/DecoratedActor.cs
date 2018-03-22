using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Actor.Base;

namespace Actor.Util
{
#if !(NETFX_CORE) 
    public class DecoratedActor : BaseActor
    {
        public DecoratedActor() : base()
        {
            // start with empty behaviors
            // reflexion on current attribute Behavior
            // add this to behaviors
            BuildFromAttributes();
        }

        private void BuildFromAttributes()
        {
            Behaviors bhvs = new Behaviors();
            // Launch reflexion
            MemberInfo[] memberInfo = GetType().GetMethods();
            foreach(var mi in memberInfo)
            {
                DecoratedAttribute deco = (DecoratedAttribute)Attribute.GetCustomAttribute(mi, typeof(DecoratedAttribute));
                if (deco != null)
                {
                    // add behavior if we can read this method
                    var parameters = ((MethodInfo)mi).GetParameters();
                    if (parameters.Length == 1)
                    {
                        if (parameters[0].ParameterType == typeof(string))
                        {
                            Behavior<string> bhv = new Behavior<string>(s => ((MethodInfo)mi).Invoke(this, new[] { s }));
                            bhvs.AddBehavior(bhv);
                        }
                    }
                }
            }
            Become(bhvs);
        }
    }
#endif
}
