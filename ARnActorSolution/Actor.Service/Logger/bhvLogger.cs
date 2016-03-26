using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Actor.Server;

namespace Actor.Base
{
    public class actLogger : BaseActor
    {
        public actLogger(string aFilename) : base()
        {
            Become(bhvLogger.CastLogger(aFilename));
            SendMessage("Logging start");
        }
    }

    public class bhvLogger : bhvBehavior<Object>, IDisposable
    {
        private string fFilename;
        private StreamWriter fStream;

        public bhvLogger()
        {
            DoInit(ActorServer.GetInstance().Name);
        }

        public static bhvLogger CastLogger(string aFilename)
        {
            bhvLogger lLogger = new bhvLogger();
            lLogger.DoInit(aFilename);
            return lLogger;
        }

        private void DoInit(string aFilename)
        {
            fFilename = Environment.CurrentDirectory + aFilename;
            fStream = new StreamWriter(fFilename, true);
            fStream.AutoFlush = true;
            Pattern = t => true;
            Apply = DoLog;
        }

        private void DoLog(Object msg)
        {
            if (msg != null)
                fStream.WriteLine(DateTimeOffset.Now.ToString()+"-"+msg.ToString());
        }

        protected virtual void Dispose(bool aDispose)
        {
            if (aDispose)
            {
                fStream.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
