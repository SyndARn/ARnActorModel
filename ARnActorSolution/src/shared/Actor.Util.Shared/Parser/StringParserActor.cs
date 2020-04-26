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
            AddBehavior(new Behavior<IActor, string, IFuture<IActor>>(
                (a, s, f) =>
                {
                    char[] chr = { ' ' };
                    var stringtoparse = s.Trim().Split(chr);
                    foreach (string item in stringtoparse)
                    {
                        a.SendMessage(this, item);
                    }
                    f.SendMessage(this);
                }
                ));
        }
        public void ParseString(IActor actor, string s)
        {
            this.SendMessage(actor, s);
        }

        public void ParseString(IActor actor, string s, IFuture<IActor> notify)
        {
            this.SendMessage(actor, s, notify);
        }
    }
}
