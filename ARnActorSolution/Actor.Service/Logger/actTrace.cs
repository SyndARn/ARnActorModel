using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Service
{
    public class actTrace : BaseActor // dummmy actor
    {
        public actTrace()
        {
            Become(null);
        }

        private static Lazy<LoggerActor> fLogger = new Lazy<LoggerActor>
            (
            () =>
            {
                return new LoggerActor("TraceLogger");
            }
            , true);

        private Stopwatch fWatch;

        public void Start()
        {
            fWatch = new Stopwatch();
            fWatch.Start() ;
        }

        public void Stop(string aMsg)
        {
            fWatch.Stop();
            fLogger.Value.SendMessage("[Trace] " + fWatch.ElapsedTicks.ToString() + " " + aMsg);
        }
    }
}
