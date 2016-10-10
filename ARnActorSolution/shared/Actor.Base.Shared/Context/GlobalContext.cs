using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.Base
{
    public static class GlobalContext
    {
        public static IMessageTracerService MessageTracerService { get; set; }
    }
}
