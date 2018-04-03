using System;
using Actor.Base;
using System.IO;

namespace Actor.Util
{

    public class TextWriterActor : BaseActor, IDisposable
    {
        private StreamWriter fStream;
#if !(NETFX_CORE) || WINDOWS_UWP
        private string fFileName;
#endif
        public TextWriterActor(string aFileName)
        {
#if !(NETFX_CORE) || WINDOWS_UWP
            Become(new Behavior<string>(DoInit));
#endif
            this.SendMessage(aFileName);
        }

        public TextWriterActor(Stream aStream)
        {
            Become(new Behavior<Stream>(DoInit));
            this.SendMessage(aStream);
        }

        public void Flush()
        {
            IFuture<bool> future = new Future<bool>();
            this.SendMessage(this,future);
            future.Result();
        }

        private void DoFlush(TextWriterActor actor, IFuture<bool> future)
        {
            fStream.Flush();
            future.SendMessage(true);
        }

        private void DoInit(Stream aStream)
        {
            fStream = new StreamWriter(aStream)
            {
                AutoFlush = true
            };
            Become(new Behavior<string>(DoWrite));
            AddBehavior(new Behavior<TextWriterActor, IFuture<bool>>(DoFlush));
        }

#if !(NETFX_CORE) || WINDOWS_UWP
        private void DoInit(string aFilename)
        {
            fFileName = aFilename;
            fStream = new StreamWriter(new FileStream(fFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                AutoFlush = true
            };
            Become(new Behavior<string>(DoWrite));
            AddBehavior(new Behavior<TextWriterActor, IFuture<bool>>(DoFlush));
        }
#endif

        private void DoWrite(string msg)
        {
            fStream.WriteLine(msg);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                if (fStream != null)
                {
                    fStream.Flush();
                    fStream.Dispose();
                    fStream = null;
                }
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}

