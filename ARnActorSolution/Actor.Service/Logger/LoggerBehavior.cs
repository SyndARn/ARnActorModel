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
                if (parent.fMessageList.Count > 0)
                {
                    using (var fStream = new StreamWriter(parent.fFilename, true))
                    {
                        parent.fMessageList.ForEach(o => fStream.WriteLine(o));
                    }
                    parent.fMessageList.Clear();
                }
            };
        }
    }



    public class LoggerBehaviors : Behaviors
    {
        public string fFilename { get; private set; }
        public List<object> fMessageList = new List<object>();


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
            fFilename = Environment.CurrentDirectory + aFilename;
            this.BecomeBehavior(new LogHeartBeatBehavior());
            AddBehavior(new Behavior<object>(msg =>
            {
                if (msg != null)
                {
                    string s = String.Format(CultureInfo.InvariantCulture, "{0:o} - {1}", DateTimeOffset.UtcNow, msg);
                    fMessageList.Add(s);
                }
            }));
        }
    }
}
