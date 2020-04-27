using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;

namespace Actor.Server
{
    [Serializable]
    public class ShardRequest
    {
        public string RequestType { get; private set; } // Ask or Answer
        public IActor Sender { get; private set; }
        public IActor Target { get; private set; }
        public IEnumerable<string> Data { get; private set; }

        public ShardRequest() { }

        public static ShardRequest CastRequest(IActor aSender, IActor aTarget)
        {
            return new ShardRequest
            {
                RequestType = "Ask",
                Sender = aSender,
                Target = aTarget
            };
        }

        public ShardRequest CastAnswer(IEnumerable<string> someData)
        {
            return new ShardRequest
            {
                RequestType = "Answer",
                Sender = this.Sender,
                Target = this.Target,
                Data = new List<string>(someData)
            };
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in Data)
            {
                sb.AppendLine(s);
            }

            return sb.ToString();
        }
    }
}
