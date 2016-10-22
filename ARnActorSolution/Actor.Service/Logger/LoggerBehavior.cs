﻿using System;
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

    public class LoggerBehavior : Behavior<Object>
    {
        private string fFilename;

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
            Apply = DoLog;
        }

        private void DoLog(object msg)
        {
            if (msg != null)
            {
                string s = String.Format("{0:o} - {1}", DateTimeOffset.UtcNow, msg);
                using (var fStream = new StreamWriter(fFilename, true))
                {
                    fStream.WriteLine(s);
                }
            }
        }
    }
}
