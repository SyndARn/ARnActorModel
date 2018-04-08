using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Actor.Base
{
    public class MemoryMessageTracerService : IMessageTracerService
    {
        private readonly ConcurrentQueue<string> fMessageTrace = new ConcurrentQueue<string>();
        public void TraceMessage(object message)
        {
            fMessageTrace.Enqueue(message == null ? "null message" : message.ToString());
        }

        public IReadOnlyList<string> CopyAllMessages()
        {
            List<string> messages = new List<string>();
            while (fMessageTrace.TryDequeue(out string aMessage))
            {
                messages.Add(aMessage);
            }
            return messages;
        }
    }
}
