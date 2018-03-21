using System.IO;
using Actor.Server;
using Actor.Base;

namespace Actor.Service
{
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
}
