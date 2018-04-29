using System;
using System.Diagnostics;
using Actor.Base;
using System.Globalization;

namespace Actor.Service
{
    public class TraceActor : BaseActor
    {
        public TraceActor()
        {
            Become(new NullBehaviors());
        }

        private static readonly Lazy<LoggerActor> fLogger = new Lazy<LoggerActor>
            (
            () => new LoggerActor("TraceLogger")
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
