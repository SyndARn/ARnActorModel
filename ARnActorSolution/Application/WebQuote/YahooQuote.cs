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
            Become(new Behavior<Tuple<string, IActor>>(DoQuote));
            SendMessage(Tuple.Create(string.Format(model, currencyPair), target));
        }

        private void DoQuote(Tuple<string, IActor> msg)
        {
            using (var client = new HttpClient())
            {
                using (var hc = new StringContent(msg.Item1))
                {
                    Uri uri = new Uri(msg.Item1);
                    var post = client.PostAsync(uri, hc).Result;
                    string result = post.Content.ReadAsStringAsync().Result;
                    msg.Item2.SendMessage(result);
                }
            }
        }
    }
}