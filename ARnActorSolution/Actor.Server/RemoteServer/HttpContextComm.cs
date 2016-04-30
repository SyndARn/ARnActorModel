using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Server
{

    class HttpContextComm : IContextComm
    {
        private HttpListenerContext fContext;
        public HttpContextComm(HttpListenerContext context)
        {
            fContext = context;
        }

        public Stream ReceiveStream()
        {
            return fContext.Request.InputStream;
        }

        public void Acknowledge()
        {
            HttpListenerResponse response = fContext.Response;
            response.Close();
        }


    }
}
