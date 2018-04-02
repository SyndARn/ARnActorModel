using Actor.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Server
{

    class HttpContextComm : IContextComm
    {
        private HttpListenerContext fContext;
        private HttpListener fListener;
        public HttpContextComm(HttpListener listener)
        {
            fListener = listener;
        }

        public Stream ReceiveStream()
        {
            fContext = fListener.GetContext(); // blocking
            return fContext.Request.InputStream;
        }

        public void Acknowledge()
        {
            if (fContext != null)
            {
                HttpListenerResponse response = fContext.Response;
                response.Close();
                fContext = null;
                fListener = null;
            }
        }

        public void SendStream(string uri, Stream stream)
        {
            CheckArg.Stream(stream);
            using (StreamReader srDebug = new StreamReader(stream))
            {
                while (!srDebug.EndOfStream)
                    Debug.Print(srDebug.ReadLine());

                stream.Seek(0, SeekOrigin.Begin);
                using (var client = new HttpClient())
                {
                    using (var hc = new StreamContent(stream))
                    {
                        client.PostAsync(uri, hc).Wait();
                    }
                }
            }
        }

    }
}
