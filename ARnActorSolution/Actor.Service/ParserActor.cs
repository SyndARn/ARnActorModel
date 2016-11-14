using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base ;
using Actor.Server ;

namespace Actor.Service
{

    public class BehaviorReceiveLine : Behavior<IActor, string>
    {
        public BehaviorReceiveLine() : base()
        {
            Pattern = DefaultPattern();
            Apply = (a,s) =>
                {
                    IActor parser = new StringParserActor();
                    parser.SendMessage(a,s);
                };
        }
    }

    public class StringParserActor : BaseActor
    {
        public StringParserActor()
            : base()
        {
            Become(new Behavior<IActor,string>(
                (a,s) => 
                    {
                        char[] chr = {' '} ;
                        var stringtoparse = s.Trim().Split(chr) ;
                        foreach (string item in stringtoparse)
                        {
                            a.SendMessage(this,item);
                        }
                    } 
                )) ;
        }
    }

    public class ParserServer : BaseActor
    {
        public ParserServer() : base()
        {
            Become(new BehaviorReceiveLine()) ;
        }
    }

    public class ParserActor : BaseActor
    {
        private List<String> fList = new List<String>();
        private ActorTag fParserServer ;
        public ParserActor()
            : base()
        {
            var collect = new Behavior<IActor,string>((a,s) =>
                {
                    fList.Add(s) ;
                    Console.WriteLine("parsed {0}",s);
                }
                );
            var connect = new Behavior<ActorTag>(t =>
                {
                    fParserServer = t;
                }
                );
            var parse = new Behavior<IEnumerable<string>>(t =>
                {
                    IActor aServer = null;
                    if (fParserServer == null)
                    {
                        aServer = new ParserServer();
                        fParserServer = aServer.Tag;
                    }
                    else
                    {
                        aServer = new ConnectActor(this, fParserServer.Host, "ParserServer");
                    }
                    foreach (string s in t)
                    {
                        aServer.SendMessage(this, s);
                    }
                }
            );
            Become(collect);
            AddBehavior(connect);
            AddBehavior(parse);
        }
    }
}
