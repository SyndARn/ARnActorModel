﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.Util
{
#if !(NETFX_CORE) 
    public class BehaviorAttribute : Attribute
    {
    }
#endif
}