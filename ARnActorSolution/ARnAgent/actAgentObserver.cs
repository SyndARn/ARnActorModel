using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARnAgent
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "act")]
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
