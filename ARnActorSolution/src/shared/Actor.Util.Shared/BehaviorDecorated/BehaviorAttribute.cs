using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.Util
{
#if !NETFX_CORE
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class BehaviorAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActionBehaviorAttribute : Attribute
    {
    }
#endif
}