using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    public class actTrace : actActor // dummmy actor
    {
        public actTrace()
        {
            Become(null);
        }

        private static Lazy<actLogger> fLogger = new Lazy<actLogger>
            (
            () =>
            {
                return new actLogger("TraceLogger");
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
