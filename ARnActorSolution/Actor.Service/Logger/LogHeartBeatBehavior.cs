using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Actor.Base;
using Actor.Server;

namespace Actor.Service
{

    public class LogHeartBeatBehavior : Behavior<HeartBeatActor, HeartBeatAction>
    {
        public LogHeartBeatBehavior() : base()
        {
            Pattern = (actor, action) => (actor is HeartBeatActor) && (action == HeartBeatAction.Beat);
            Apply = (actor, action) =>
                {
                    LoggerBehaviors parent = LinkedTo as LoggerBehaviors;
                    if (parent.MessageList.Count <= 0)
                    {
                        return;
                    }

                    using (StreamWriter fStream = new StreamWriter(parent.FileName, true))
                    {
                        parent.MessageList.ForEach(o => fStream.WriteLine(o));
                    }

                    parent.MessageList.Clear();
                }
;
        }
    }
}
