using System;
using Actor.Base;
using System.IO;

namespace Actor.Util
{
    public class TextWriterActor : BaseActor, IDisposable
    {
        private StreamWriter _stream;
#if !(NETFX_CORE) || WINDOWS_UWP
        private string _fileName;
#endif
        public TextWriterActor(string aFileName)
        {
#if !(NETFX_CORE) || WINDOWS_UWP
            Become(new Behavior<string>(DoInit));
#endif
            SendMessage(aFileName);
        }

        public TextWriterActor(Stream aStream)
        {
            Become(new Behavior<Stream>(DoInit));
            SendMessage(aStream);
        }

        public void Flush()
        {
            IFuture<bool> future = new Future<bool>();
            this.SendMessage(this, future);
            future.Result();
        }

        private void DoFlush(TextWriterActor actor, IFuture<bool> future)
        {
            _stream.Flush();
            future.SendMessage(true);
        }

        private void DoInit(Stream aStream)
        {
            _stream = new StreamWriter(aStream)
            {
                AutoFlush = true
            };
            Become(new Behavior<string>(DoWrite));
            AddBehavior(new Behavior<TextWriterActor, IFuture<bool>>(DoFlush));
        }

#if !(NETFX_CORE) || WINDOWS_UWP
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Supprimer les objets avant la mise hors de portée")]
        private void DoInit(string aFilename)
        {
            _fileName = aFilename;
            _stream = new StreamWriter(new FileStream(_fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                AutoFlush = true
            };
            Become(new Behavior<string>(DoWrite));
            AddBehavior(new Behavior<TextWriterActor, IFuture<bool>>(DoFlush));
        }
#endif

        private void DoWrite(string msg) => _stream.WriteLine(msg);

        protected virtual void Dispose(bool disposing)
            {
            if (!disposing || (_stream == null))
            {
                return;
            }

            _stream.Flush();
            _stream.Dispose();
            _stream = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

