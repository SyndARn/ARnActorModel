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
        public string Url { get; set; }
    }

    public class WebAnswer
    {
        public string Url { get; set; }
        public string Answer { get; set; }
    }

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
            if (aWebRequest.Url != "")
            {
                HttpClient client = new HttpClient();
                string s = client.GetStringAsync(aWebRequest.Url).Result;
                var ans = new WebAnswer();
                ans.Url = aWebRequest.Url;
                ans.Answer = s;
                aWebRequest.Sender.SendMessage(ans);
            }
        }

    }

    public class actActorWeb : actActor
    {
        public actActorWeb()
        {
            Become(new bhvWebClient());
        }
        public static WebRequest Cast(IActor aSender, string anUrl)
        {
            return new WebRequest() { Sender = aSender, Url = anUrl };
        }
    }
}
