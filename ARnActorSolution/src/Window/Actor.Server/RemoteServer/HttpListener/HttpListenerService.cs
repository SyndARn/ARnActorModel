using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;

namespace Actor.Server
{
    public class HttpListenerService : IListenerService, IDisposable
    {
        private HttpListener _listener;

        public HttpListenerService()
        {
            _listener = new HttpListener();
            StartServer();
        }

        public IContextComm GetCommunicationContext() => new HttpContextComm(_listener);

        public void Close() => _listener.Close();

        private void StartServer()
        {
            string localhost = Dns.GetHostName();
            string servername = ActorServer.GetInstance().Name;
            const string prefix = "http://";
            string suffix = ":" + ActorServer.GetInstance().Port.ToString(CultureInfo.InvariantCulture);
            _listener.Prefixes.Add(prefix + "localhost" + suffix + "/" + servername + "/");
            _listener.Prefixes.Add(prefix + localhost + suffix + "/" + servername + "/");
            _listener.Prefixes.Add(prefix + "127.0.0.1" + suffix + "/" + servername + "/");
            try
            {
                _listener.Start();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Can't start http " + e);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // Pour détecter les appels redondants

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_listener != null)
                    {
                        ((IDisposable)_listener).Dispose();
                        _listener = null;
                    }
                }

                // TODO: libérer les ressources non managées (objets non managés) et remplacer un finaliseur ci-dessous.
                // TODO: définir les champs de grande taille avec la valeur Null.

                disposedValue = true;
            }
        }

        // TODO: remplacer un finaliseur seulement si la fonction Dispose(bool disposing) ci-dessus a du code pour libérer les ressources non managées.
        // ~HttpListenerService() {
        //   // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
        //   Dispose(false);
        // }

        // Ce code est ajouté pour implémenter correctement le modèle supprimable.
        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(true);
            // TODO: supprimer les marques de commentaire pour la ligne suivante si le finaliseur est remplacé ci-dessus.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
