using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Actor.Base;
using Actor.Server;

namespace Actor.Service
{
    public class LoggerActor : BaseActor
    {
        public LoggerActor(string aFileName) : base()
        {
            Become(new LoggerBehaviors(aFileName));
            var heartBeat = new HeartBeatActor(500);
            heartBeat.SendMessage((IActor)this);
            SendMessage("Logging start");
        }
    }
}
