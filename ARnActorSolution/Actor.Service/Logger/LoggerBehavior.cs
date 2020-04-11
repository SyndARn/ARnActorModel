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
            Become(new LoggerBehaviors(aFileName));
            var heartBeat = new HeartBeatActor(500);
            heartBeat.SendMessage((IActor)this);
            this.SendMessage("Logging start");
        }
    }

    public class LogHeartBeatBehavior : Behavior<HeartBeatActor, HeartBeatAction>
    {
        public LogHeartBeatBehavior() : base()
        {
            Pattern = (actor, action) => (actor is HeartBeatActor) && (action == HeartBeatAction.Beat);
            Apply = (actor,action) =>
            {
                var parent = LinkedTo as LoggerBehaviors;
                if (parent.MessageList.Count > 0)
                {
                    using (var fStream = new StreamWriter(parent.FileName, true))
                    {
                        parent.MessageList.ForEach(o => fStream.WriteLine(o));
                    }
                    parent.MessageList.Clear();
                }
            };
        }
    }

    public class LoggerBehaviors : Behaviors
    {
        public string FileName { get; private set; }
        public List<object> MessageList { get; } = new List<object>();

        public LoggerBehaviors() : base()
        {
            DoInit(ActorServer.GetInstance().Name);
        }

        public LoggerBehaviors(string fileName) : base()
        {
            DoInit(fileName);
        }

        private void DoInit(string fileName)
        {
            FileName = Path.Combine(Environment.CurrentDirectory,fileName);
            BecomeBehavior(new LogHeartBeatBehavior());
            AddBehavior(new Behavior<string>(msg =>
            {
                if (msg != null)
                {
                    string s = String.Format(CultureInfo.InvariantCulture, "{0:o} - {1}", DateTimeOffset.UtcNow, msg);
                    MessageList.Add(s);
                }
            }));
        }
    }
}
