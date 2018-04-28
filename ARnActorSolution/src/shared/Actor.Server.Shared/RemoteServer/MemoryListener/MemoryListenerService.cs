using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Server
{
    public class MemoryListenerService : IListenerService
    {
        private MemoryContextComm  memoryContext = new MemoryContextComm();

        public void Close()
        {
            // TODO nothing to do ?
            // throw new NotImplementedException();
        }

        public IContextComm GetCommunicationContext()
        {
            return memoryContext;
        }
    }
}
