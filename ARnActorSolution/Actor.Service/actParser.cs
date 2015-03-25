using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base ;
using Actor.Util ;

namespace Actor.Service
{

    public class bhvReceiveLine : bhvBehavior<Tuple<IActor,String>>
    {
        public bhvReceiveLine() : base()
        {
            Pattern = t => t is Tuple<IActor,String> ;
            Apply = t =>
                {
                    IActor parser = new actStringParser();
                    SendMessageTo(t, parser);
                };
        }
    }

    public class actStringParser : actActor
    {
        public actStringParser()
            : base()
        {
            Become(new bhvBehavior<Tuple<IActor,string>>(
                t => 
                    {
                        char[] chr = {' '} ;
                        var stringtoparse = t.Item2.Trim().Split(chr) ;
                        foreach (string s in stringtoparse)
                        {
                            SendMessageTo(new Tuple<IActor,string>(this,s), t.Item1);
                        }
                    } 
                )) ;
        }
    }

    public class actParserServer : actActor
    {
        public actParserServer() : base()
        {
            Become(new bhvReceiveLine()) ;
        }
    }

    public class actParser : actActor
    {
        private List<String> fList = new List<String>();
        private actTag fParserServer ;
        public actParser()
            : base()
        {
            var collect = new bhvBehavior<Tuple<IActor,string>>(t =>
                {
                    fList.Add(t.Item2) ;
                    Console.WriteLine("parsed "+t.Item2);
                }
                );
            var connect = new bhvBehavior<actTag>(t =>
                {
                    fParserServer = t;
                }
                );
            var parse = new bhvBehavior<IEnumerable<string>>(t =>
                {
                    IActor aServer = null;
                    if (fParserServer == null)
                    {
                        aServer = new actParserServer();
                        fParserServer = aServer.Tag;
                    }
                    else
                    {
                        aServer = new actConnect(this, fParserServer.Uri, "ParserServer");
                    }
                    foreach (string s in t)
                    {
                        SendMessageTo(new Tuple<IActor,string>(this,s), aServer);
                    }
                }
            );
            Become(collect);
            AddBehavior(connect);
            AddBehavior(parse);
        }
    }
}
