using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    // hold the pattern to be used in receive mode
    // protected by fReceiveMode
    internal class PatternFuture
    {
        public Func<Object, bool> Pattern;
        public TaskCompletionSource<Object> TaskCompletion;
        public Object Message;
    }
}
