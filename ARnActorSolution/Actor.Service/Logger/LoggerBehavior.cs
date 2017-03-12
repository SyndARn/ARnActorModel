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
            heartBeat.SendMessage(this);
            SendMessage("Logging start");
        }
    }

    public class LogHeartBeatBehavior : Behavior<IMessageParam<HeartBeatActor, HeartBeatAction>>
    {
        public LogHeartBeatBehavior() : base()
        {
            Apply = msgprm =>
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
        private List<object> fMessageList = new List<object>();
        public List<object> MessageList { get { return fMessageList; } }


        public LoggerBehaviors() : base()
        {
            DoInit(ActorServer.GetInstance().Name);
        }
        public LoggerBehaviors(string aFilename) : base()
        {
            DoInit(aFilename);
        }
        private void DoInit(string aFilename)
        {
            FileName = Environment.CurrentDirectory + aFilename;
            this.BecomeBehavior(new LogHeartBeatBehavior());
            AddBehavior(new Behavior<object>(msg =>
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
