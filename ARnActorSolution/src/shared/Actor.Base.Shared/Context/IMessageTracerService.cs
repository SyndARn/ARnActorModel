﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.Base
{
    public interface IMessageTracerService
    {
        void TraceMessage(Object message);
        IReadOnlyList<string> CopyAllMessages();
    }
}
