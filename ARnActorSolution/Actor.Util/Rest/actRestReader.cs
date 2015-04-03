using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Diagnostics;

namespace Actor.Util
{
    public class actRestReader : actActor
    {
        public actRestReader() : base()
        {
            BecomeMany(new bhvRestReader()) ;
        }

        public void SendRest(Uri anUri, IActor answer) 
        {
            SendMessageTo(Tuple.Create(anUri, answer));
        }
    }

    public class bhvRestReader : Behaviors
    {
        public IActor fAnswer;
        public bhvRestReader() : base()
        {
            this.AddBehavior(new bhvRestSend()) ;
            this.AddBehavior(new bhvRestReceive()) ;
        }
    }

    public class bhvRestReceive : bhvBehavior<WebAnswer>
    {
        public bhvRestReceive() : base()
        {
            Pattern = t => t is WebAnswer ;
            Apply = DoRestReceive;
        }
        private void DoRestReceive(WebAnswer webAnswer)
        {
            Debug.WriteLine("Receive {0}",webAnswer.Answer) ;
            var reader = this.LinkedTo() as bhvRestReader;
            SendMessageTo(webAnswer.Answer, reader.fAnswer);
        }
    }

    public class bhvRestSend : bhvBehavior<Tuple<Uri,IActor>>
    {
        public bhvRestSend()
            : base()
        {
            Pattern = t => t is Tuple<Uri,IActor>;
            Apply = DoRestPost;
        }
        private void DoRestPost(Tuple<Uri,IActor> anUri)
        {
            (this.LinkedTo() as bhvRestReader).fAnswer = anUri.Item2;
            var actWeb = new actActorWeb();
            var wr = actActorWeb.Cast(this.LinkedTo().LinkedActor, anUri.Item1.AbsoluteUri);
            SendMessageTo(wr, actWeb);
        }
    }
}
