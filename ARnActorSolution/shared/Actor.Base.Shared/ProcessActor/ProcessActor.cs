using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Actor.Base
{
    public class ProcessActor<T> : BaseActor
    {
        public ProcessActor(Action<T> anAction)
        {
            Become(new Behavior<T>(anAction));
        }
    }
}
