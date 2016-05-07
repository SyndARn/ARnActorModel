using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Actor.Service
{
    public class RSSReaderActor : BaseActor
    {
        string fUrl;
        public RSSReaderActor(string anUrl, IActor target)
        {
            fUrl = anUrl;
            Become(new Behavior<Tuple<string, IActor>>(DoSyndication));
            SendMessage(Tuple.Create(anUrl, target));
        }

        private void DoSyndication(Tuple<string, IActor> msg)
        {
            XmlReader reader = XmlReader.Create(msg.Item1);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();
            foreach (SyndicationItem item in feed.Items)
            {
                msg.Item2.SendMessage(item.Title.Text);
            }
        }
    }
}
