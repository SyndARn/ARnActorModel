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
    public class RssReaderActor : BaseActor
    {
        public RssReaderActor(string url, IActor target)
        {
            Become(new Behavior<string, IActor>(DoSyndication));
            this.SendMessage(url, target);
        }

        private void DoSyndication(string url, IActor target)
        {
            XmlReader reader = null;
            try
            {
                reader = XmlReader.Create(url);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();
                foreach (SyndicationItem item in feed.Items)
                {
                    target.SendMessage(item.Title.Text);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
            }
        }
    }
}
