using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;

namespace Actor.Server
{
    public class HttpListenerService : IListenerService, IDisposable
    {
        private HttpListener fListener;

        public HttpListenerService()
        {
            fListener = new HttpListener();
            StartServer();
        }

        public IContextComm GetCommunicationContext()
        {
            return new HttpContextComm(fListener);
        }

        public void Close()
        {
            fListener.Close();
        }

        private void StartServer()
        {
            var localhost = Dns.GetHostName();
            var servername = ActorServer.GetInstance().Name;
            const string prefix = "http://";
            var suffix = ":" + ActorServer.GetInstance().Port.ToString(CultureInfo.InvariantCulture);
            fListener.Prefixes.Add(prefix + "localhost" + suffix + "/" + servername + "/");
            fListener.Prefixes.Add(prefix + localhost + suffix + "/" + servername + "/");
            fListener.Prefixes.Add(prefix + "127.0.0.1" + suffix + "/" + servername + "/");
            try
            {
                fListener.Start();
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
                    if (fListener != null)
                    {
                        ((IDisposable)fListener).Dispose();
                        fListener = null;
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
