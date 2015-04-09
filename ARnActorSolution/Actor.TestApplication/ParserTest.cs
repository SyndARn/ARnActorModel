using Actor.Base;
using Actor.Service;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.TestApplication
{
    public class ParserTest : actActionActor
    {
        public ParserTest()
            : base()
        {
            SendAction(DoParser);
        }

        private void DoParser()
        {
            List<String> aList = new List<String>();
            aList.Add(ActorTask.Stat());
            var lParser = new actParser();
            new actEchoActor<IEnumerable<string>>(lParser, aList);
            lParser.SendMessage(aList.AsEnumerable<String>());
        }
    }
}
