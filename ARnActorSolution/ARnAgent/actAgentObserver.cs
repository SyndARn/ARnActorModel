using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARnAgent
{
    public class actAgentObserver : actActor
    {
        public actAgentObserver() : base()
        {
            Become(new bhvBehavior<string>(OnObserve));
        }

        private void OnObserve(string msg)
        {
            if (! string.IsNullOrEmpty(msg))
              Console.WriteLine(msg);
        }

    }
}
