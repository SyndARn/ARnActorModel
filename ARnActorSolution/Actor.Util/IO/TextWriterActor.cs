using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.IO;

namespace Actor.Util
{
    public class TextWriterActor : actActor
    {
        private StreamWriter fStream;
        private string fFileName;

        public TextWriterActor(string aFileName)
        {
            Become(new Behavior<string>(DoInit));
            SendMessage(aFileName);
        }

        private void DoInit(string aFilename)
        {
            fFileName = Environment.CurrentDirectory + aFilename;
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
            GC.SuppressFinalize(this);
        }
    }

}

