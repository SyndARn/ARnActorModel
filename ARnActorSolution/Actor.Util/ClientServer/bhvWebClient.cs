using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Net.Http;

namespace Actor.Util
{
    public class WebRequest
    {
        public IActor Sender { get; set; }
        public Uri Url { get; set; }
        public WebAnswer CastAnswer(string s)
        {
            return WebAnswer.Cast(Url,s) ;
        }
    }

    public class WebAnswer
    {
        public Uri Url { get; private set; }
        public string Answer { get; private set; }
        public static WebAnswer Cast(Uri anUri, string anAnswer)
        {
            return new WebAnswer()
            {
                Url = anUri,
                Answer = anAnswer
            };
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "bhv")]
    public class bhvWebClient : bhvBehavior<WebRequest>
    {
        public bhvWebClient()
            : base()
        {
            Pattern = t => t is WebRequest;
            Apply = DoWebRequestApply;
        }

        private void DoWebRequestApply(WebRequest aWebRequest)
        {
            if (aWebRequest.Url != null)
            {
                using (HttpClient client = new HttpClient())
                {
                    string s = client.GetStringAsync(aWebRequest.Url).Result;
                    aWebRequest.Sender.SendMessage(aWebRequest.CastAnswer(s));
                }
            }
        }

    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "act")]
    public class actActorWeb : BaseActor
    {
        public actActorWeb()
        {
            Become(new bhvWebClient());
        }
        public static WebRequest Cast(IActor aSender, Uri anUrl)
        {
            return new WebRequest() { Sender = aSender, Url = anUrl };
        }
    }
}
