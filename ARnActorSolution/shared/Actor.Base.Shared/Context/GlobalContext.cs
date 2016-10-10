using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.Base
{
    internal static class GlobalContext
    {
        public static IMessageTracerService MessageTracerService { get; set; }
    }
}
