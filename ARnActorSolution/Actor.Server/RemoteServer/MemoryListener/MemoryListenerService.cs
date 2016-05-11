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

        public void Close()
        {
            // TODO nothing to do ?
            // throw new NotImplementedException();
        }

        public IContextComm GetCommunicationContext()
        {
            return new MemoryContextComm();
        }
    }
}
