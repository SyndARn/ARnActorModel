using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Server
{
    public interface IListenerService
    {
        IContextComm GetCommunicationContext();
        void Close();
    }
}
