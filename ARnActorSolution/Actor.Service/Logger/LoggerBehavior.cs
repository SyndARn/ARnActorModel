using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Actor.Base;
using Actor.Server;

namespace Actor.Service
{
    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class LoggerBehaviors : Behaviors
    {
        public string FileName { get; private set; }
        public List<object> MessageList { get; } = new List<object>();

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => ToString();

        public LoggerBehaviors() : base() => DoInit(ActorServer.GetInstance().Name);

        public LoggerBehaviors(string fileName) : base() => DoInit(fileName);

        private void DoInit(string fileName)
        {
            FileName = Path.Combine(Environment.CurrentDirectory, fileName);
            BecomeBehavior(new LogHeartBeatBehavior());
            AddBehavior(new Behavior<string>(msg =>
            {
                if (msg != null)
                {
                    string s = string.Format(CultureInfo.InvariantCulture, "{0:o} - {1}", DateTimeOffset.UtcNow, msg);
                    MessageList.Add(s);
                }
            }));
        }
    }
}
