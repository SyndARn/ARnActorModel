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
            SendMessage(Tuple.Create(anUri, answer));
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
            Pattern = DefaultPattern();
            Apply = DoRestReceive;
        }
        private void DoRestReceive(WebAnswer webAnswer)
        {
            Debug.WriteLine("Receive {0}",webAnswer.Answer) ;
            var reader = this.LinkedTo as bhvRestReader;
            reader.fAnswer.SendMessage(webAnswer.Answer);
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
            (LinkedTo as bhvRestReader).fAnswer = anUri.Item2;
            var actWeb = new actActorWeb();
            var wr = actActorWeb.Cast(LinkedActor, anUri.Item1.AbsoluteUri);
            actWeb.SendMessage(wr);
        }
    }
}
