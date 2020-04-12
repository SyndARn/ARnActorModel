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
        private HttpListenerContext _context;
        private HttpListener _listener;
        public HttpContextComm(HttpListener listener) => _listener = listener;

        public Stream ReceiveStream()
        {
            _context = _listener.GetContext(); // blocking
            return _context.Request.InputStream;
        }

        public void Acknowledge()
        {
            if (_context != null)
            {
                HttpListenerResponse response = _context.Response;
                response.Close();
                _context = null;
                _listener = null;
            }
        }

        public void SendStream(string uri, Stream stream)
        {
            CheckArg.Stream(stream);
            using (StreamReader srDebug = new StreamReader(stream))
            {
                while (!srDebug.EndOfStream)
                {
                    Debug.Print(srDebug.ReadLine());
                }

                stream.Seek(0, SeekOrigin.Begin);
                using (HttpClient client = new HttpClient())
                {
                    using (StreamContent hc = new StreamContent(stream))
                    {
                        client.PostAsync(uri, hc).Wait();
                    }
                }
            }
        }

    }
}
