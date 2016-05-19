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
        private string fFileName;

        public TextWriterActor(string aFileName)
        {
            Become(new Behavior<string>(DoInit));
            AddBehavior(new Behavior<TextWriterActor>(DoFlush));
            SendMessage(aFileName);
        }

        public TextWriterActor(Stream aStream)
        {
            Become(new Behavior<Stream>(DoInit));
            AddBehavior(new Behavior<TextWriterActor>(DoFlush));
            SendMessage(aStream);
        }

        public void Flush()
        {
            SendMessage(this);
        }

        private void DoFlush(TextWriterActor actor)
        {
            fStream.Close();
        }

        private void DoInit(Stream aStream)
        {
            fStream = new StreamWriter(aStream);
            fStream.AutoFlush = true;
            Become(new Behavior<string>(DoWrite));
        }

        private void DoInit(string aFilename)
        {
            fFileName = aFilename;
            fStream = new StreamWriter(fFileName, true);
            fStream.AutoFlush = true;
            Become(new Behavior<string>(DoWrite));
        }

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
                    fStream.Close();
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

