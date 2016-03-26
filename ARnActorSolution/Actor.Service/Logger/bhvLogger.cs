using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Actor.Server;
using Actor.Base;

namespace Actor.Service
{
    public class LoggerActor : BaseActor
    {
        public LoggerActor(string aFilename) : base()
        {
            Become(LoggerBehavior.CastLogger(aFilename));
            SendMessage("Logging start");
        }
    }

    public class LoggerBehavior : Behavior<Object>, IDisposable
    {
        private string fFilename;
        private StreamWriter fStream;

        public LoggerBehavior()
        {
            DoInit(ActorServer.GetInstance().Name);
        }

        public static LoggerBehavior CastLogger(string aFilename)
        {
            LoggerBehavior lLogger = new LoggerBehavior();
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
