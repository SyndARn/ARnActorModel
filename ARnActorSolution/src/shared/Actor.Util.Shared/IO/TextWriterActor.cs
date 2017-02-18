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
            Become(new Behavior<TextWriterActor>(DoFlush));
#if !(NETFX_CORE) || WINDOWS_UWP
            AddBehavior(new Behavior<string>(DoInit));
#endif
            this.SendMessage(aFileName);
        }

        public TextWriterActor(Stream aStream)
        {
            Become(new Behavior<TextWriterActor>(DoFlush));
            AddBehavior(new Behavior<Stream>(DoInit));
            this.SendMessage(aStream);
        }

        public void Flush()
        {
            this.SendMessage(this);
        }

        private void DoFlush(TextWriterActor actor)
        {
            fStream.Flush();
        }

        private void DoInit(Stream aStream)
        {
            fStream = new StreamWriter(aStream);
            fStream.AutoFlush = true;
            Become(new Behavior<string>(DoWrite));
        }

#if !(NETFX_CORE) || WINDOWS_UWP
        private void DoInit(string aFilename)
        {
            fFileName = aFilename;
            fStream = new StreamWriter(new FileStream(fFileName, FileMode.Create,FileAccess.ReadWrite,FileShare.Read));
            fStream.AutoFlush = true;
            Become(new Behavior<string>(DoWrite));
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

