using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{

    public class RedirectMessage
    {
        public RedirectMessage(Object aData, IActor aTarget)
        {
            Data = aData;
            Target = aTarget;
        }
        public Object Data {get ; private set;}
        public IActor Target { get; private set; }
    }

    public class actRedirector : actActor
    {
        //
        private IActor fSource = null;

        public actRedirector(IActor anActor)
            : base()
        {
            fSource = anActor;
            Become(new bhvBehavior<RedirectMessage>(DoRedirect));
        }

        private void DoRedirect(RedirectMessage aRedirection)
        {
            SendMessageTo(aRedirection.Data, aRedirection.Target);
        }
    }
}
