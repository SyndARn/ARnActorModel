using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebQuote
{
    public class YahooQuote : BaseActor
    {
        const string model = @"http://download.finance.yahoo.com/d/quotes.csv?e=.csv&f=c4l1&s={0}=X";
        public YahooQuote(string currencyPair, IActor target)
        {
            Become(new Behavior<string, IActor>(DoQuote));
            this.SendMessage(string.Format(model, currencyPair), target);
        }

        private void DoQuote(string data, IActor actor)
        {
            using (var client = new HttpClient())
            {
                using (var hc = new StringContent(data))
                {
                    Uri uri = new Uri(data);
                    var post = client.PostAsync(uri, hc).Result;
                    string result = post.Content.ReadAsStringAsync().Result;
                    actor.SendMessage(result);
                }
            }
        }
    }
}