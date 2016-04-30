using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Server
{ 
    public interface IContextComm
    {
        Stream ReceiveStream();
        void Acknowledge();
    }
}
