using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Actor.Server;
using Actor.Base;
using System.Globalization;

namespace Actor.Service
{
    public class LoggerActor : BaseActor
    {
        public LoggerActor(string aFileName) : base()
        {
            Become(LoggerBehavior.CastLogger(aFileName));
            SendMessage("Logging start");
        }
    }

    public class LoggerBehavior : Behavior<object>
    {
        private string fFilename;
        private List<Object> fMessageList = new List<object>();

        public LoggerBehavior() : base()
        {
            DoInit(ActorServer.GetInstance().Name);
        }

        private LoggerBehavior(string aFileName) : base()
        {
            DoInit(aFileName);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Supprimer les objets avant la mise hors de portée")]
        public static LoggerBehavior CastLogger(string aFileName)
        {
            LoggerBehavior lLogger = new LoggerBehavior(aFileName);
            return lLogger;
        }

        private void DoInit(string aFilename)
        {
            fFilename = Environment.CurrentDirectory + aFilename;
            Pattern = t => true;
            Apply = StartHeartBeat;
        }

        private void StartHeartBeat(object msg)
        {
            Apply = DoLog;
            var heartBeat = new HeartBeatActor(500);
            heartBeat.SendMessage(LinkedActor);
        }

        private void DoLog(object msg)
        {
            // heartbeat
            if (msg is IMessageParam<HeartBeatActor,HeartBeatAction>)
            {
                if (fMessageList.Count > 0)
                {
                    using (var fStream = new StreamWriter(fFilename, true))
                    {
                        fMessageList.ForEach(o => fStream.WriteLine(o));
                    }
                    fMessageList.Clear();
                }
                return;
            }
            if (msg != null)
            {
                string s = String.Format(CultureInfo.InvariantCulture,"{0:o} - {1}", DateTimeOffset.UtcNow, msg);
                fMessageList.Add(s);
            }
        }
    }
}
