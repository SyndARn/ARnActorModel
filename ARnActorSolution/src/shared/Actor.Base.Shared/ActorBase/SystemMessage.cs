using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#if NETCOREAPP1_1
using System.Runtime.Serialization.Json;
#endif
#if !NETCOREAPP1_1
using System.Runtime.Serialization;
#endif

namespace Actor.Base
{
    [Serializable]
    public enum SystemMessage { NullBehavior };
}
