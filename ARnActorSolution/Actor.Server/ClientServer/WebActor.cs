using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Net.Http;

namespace Actor.Server
{
    public class WebRequest
    {
        public IActor Sender { get; set; }
        public Uri Url { get; set; }
        public WebAnswer CastAnswer(string message)
        {
            return WebAnswer.Cast(Url, message) ;
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

    public class WebClientBehavior : Behavior<WebRequest>
    {
        public WebClientBehavior()
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

    public class WebActor : BaseActor
    {
        public WebActor()
        {
            Become(new WebClientBehavior());
        }
        public static WebRequest Cast(IActor aSender, Uri anUrl)
        {
            return new WebRequest() { Sender = aSender, Url = anUrl };
        }
    }
}
