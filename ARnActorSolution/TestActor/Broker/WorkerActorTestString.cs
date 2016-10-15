using Actor.Server;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Server.Tests
{
    class WorkerActorTestString : WorkerActor<string>
    {
        EnumerableActor<string> fActorReport;
        public WorkerActorTestString(EnumerableActor<string> actor)
        {
            fActorReport = actor;
        }
        protected override void Process(string aT)
        {
            fActorReport.Add(aT);
        }
    }
}
