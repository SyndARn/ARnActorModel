using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base ;

namespace Actor.Util
{
    public class actDiscovery : actActor
    {
        public actDiscovery(string lUrl)
            : base()
        {
            Become(new bhvBehavior<string>(t => {return true ;},
                Disco)) ;
            SendMessageTo(lUrl) ;
        }

        private void Disco(string lUrl)
        {
            Become(new bhvBehavior<List<String>>(t => {return true ;}, 
                Found)) ;
            var rem = new actRemoteActor(new actTag(lUrl));
            SendMessageTo(new DiscoCommand(this),rem);
        }

        private void Found(List<String> aList)
        {
            Console.WriteLine("Disco found:");
            foreach(string s in aList)
             Console.WriteLine(s);
            Become(null);
        }
    }

    public class actConnect : actActor
    {
        private string fServiceName;
        private string fUri;
        private IActor fSender;
        public actConnect(IActor lSender,string lUrl,string name) : base()
        {
            fUri = lUrl ;
            fServiceName = name ;
            fSender = lSender;
            Become(new bhvBehavior<string>(DoDisco)) ;
            SendMessageTo("DoConnect") ;
        }

        private void DoDisco(string msg)
        {
            Become(new bhvBehavior<List<String>>(Found));
            IActor rem = new actRemoteActor(new actTag(fUri));
            SendMessageTo(new DiscoCommand(this),rem);
        }

        private void Found(List<String> someServices)
        {
            char[] separator = {'='} ;
            var keyserv = someServices.ToLookup(
                s => s.Split(separator)[0],
                s => s.Split(separator)[1]) ;
            var service = keyserv[fServiceName].FirstOrDefault();
            if (!string.IsNullOrEmpty(service))
            {
                actTag tag = new actTag(service) ;
                Become(new bhvBehavior<actTag>(DoConnect));
                SendMessageTo(tag);
            }
            else
            // not found
            {
                Become(null);
            }
        }

        private void DoConnect(actTag tag)
        {
            IActor remoteSend = new actRemoteActor(tag);
            SendMessageTo(new Tuple<string,actTag,IActor>(fServiceName, tag, remoteSend),fSender);
        }

    }
}
