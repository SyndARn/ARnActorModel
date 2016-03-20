using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ActorWheather
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "act")]
    public class actStringCatcher : actActor
    {

        private string fString;
        private Object fLock = new Object();
        public string value
        {
            get
            {
                lock (fLock)
                {
                    return fString;
                }
            }
            set
            {
                lock (fLock)
                {
                    fString = value;
                }
            }
        }

        public actStringCatcher()
            : base()
        {
            this.Become(new bhvBehavior<string>(
            t => { return t is string; },
            DoCatched
            ));
        }

        private void DoCatched(string msg)
        {
            value = msg;
        }

    }
}
