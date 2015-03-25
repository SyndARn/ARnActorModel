using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.IO;

namespace Actor.Util
{
    public class actLogger : actActor
    {
        public actLogger(string aFilename) : base()
        {
            Become(bhvLogger.CastLogger(aFilename));
            SendMessageTo("Logging start");
        }
    }

    public class bhvLogger : bhvBehavior<Object>, IDisposable
    {
        private string fFilename;
        private StreamWriter fStream;

        public bhvLogger()
        {
            this.DoInit(ActorServer.GetInstance().Name);
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
            Pattern = t => true;
            Apply = DoLog;
        }

        private void DoLog(Object msg)
        {
            if (msg != null)
                fStream.WriteLine(DateTimeOffset.Now.ToString()+"-"+msg.ToString());
        }

        public void Dispose()
        {
            fStream.Dispose();
        }
    }
}
