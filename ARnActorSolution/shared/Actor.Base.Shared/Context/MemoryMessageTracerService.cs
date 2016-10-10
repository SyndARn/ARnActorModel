using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Actor.Base
{
    public class MemoryMessageTracerService : IMessageTracerService
    {
        private ConcurrentQueue<string> fMessageTrace = new ConcurrentQueue<string>();
        public void TraceMessage(object message)
        {
            fMessageTrace.Enqueue(message == null ? "null message" : message.ToString());
        }
        public IEnumerable<string> GetMessages()
        {
            string aMessage;
            while (fMessageTrace.TryDequeue(out aMessage))
            {
                yield return aMessage;
            }
        }
    }
}
