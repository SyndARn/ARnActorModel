using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;

namespace Actor.Util
{
    public class StringParserActor : BaseActor
    {
        public StringParserActor()
            : base()
        {
            Become(new Behavior<IActor, string>(
                (a, s) =>
                {
                    char[] chr = { ' ' };
                    var stringtoparse = s.Trim().Split(chr);
                    foreach (string item in stringtoparse)
                    {
                        a.SendMessage(this, item);
                    }
                }
                ));
        }

        public void ParseString(IActor actor, string s)
        {
            this.SendMessage(actor, s);
        }
    }
}
