using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            SendMessage(aFileName);
        }

        public TextWriterActor(Stream aStream)
        {
            Become(new Behavior<TextWriterActor>(DoFlush));
            AddBehavior(new Behavior<Stream>(DoInit));
            SendMessage(aStream);
        }

        public void Flush()
        {
            SendMessage(this);
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
            fStream = new StreamWriter(new FileStream(fFileName, FileMode.Create));
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
        }
    }

}

