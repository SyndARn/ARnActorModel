using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Globalization;

namespace Actor.Service
{
    public class TraceActor : BaseActor 
    {
        public TraceActor()
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
            fLogger.Value.SendMessage(String.Format(CultureInfo.InvariantCulture,"[Trace] {0} {1}", fWatch.ElapsedTicks,aMsg));
        }
    }
}
