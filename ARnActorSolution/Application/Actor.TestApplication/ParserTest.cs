using Actor.Base;
using Actor.Service;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Actor.TestApplication
{
    public class ParserTest : ActionActor
    {
        public ParserTest()
            : base()
        {
            SendAction(DoParser);
        }

        private void DoParser()
        {
            List<String> aList = new List<String>
            {
                ActorTask.Stat()
            };
            var lParser = new ParserActor();
            new EchoActor<IEnumerable<string>>(lParser, aList);
            lParser.SendMessage(aList.AsEnumerable());
        }
    }
}
