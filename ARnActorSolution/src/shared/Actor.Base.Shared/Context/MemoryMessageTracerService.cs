using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Actor.Base
{
    public class MemoryMessageTracerService : IMessageTracerService
    {
        private readonly ConcurrentQueue<string> _messageTrace = new ConcurrentQueue<string>();
        public void TraceMessage(object message)
        {
            _messageTrace.Enqueue(message == null ? "null message" : message.ToString());
        }

        public IReadOnlyList<string> CopyAllMessages()
        {
            List<string> messages = new List<string>();
            while (_messageTrace.TryDequeue(out string aMessage))
            {
                messages.Add(aMessage);
            }
            return messages;
        }
    }
}
