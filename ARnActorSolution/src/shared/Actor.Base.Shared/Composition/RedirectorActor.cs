using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    public class RedirectMessage
    {
        public Object Data { get; }
        public IActor Target { get; }

        public RedirectMessage(Object aData, IActor aTarget)
        {
            Data = aData;
            Target = aTarget;
        }
    }

    public class RedirectorActor : BaseActor
    {
        private readonly IActor fTarget = null;

        public RedirectorActor(IActor anActor)
            : base()
        {
            fTarget = anActor;
            Become(new Behavior<RedirectMessage>(DoRedirect));
        }

        private void DoRedirect(RedirectMessage aRedirection)
        {
            fTarget.SendMessage(aRedirection.Data);
        }
    }

    public class CompositionActor<T> : BaseActor
    {
        public CompositionActor()
        {
            Become(new Behavior<IFuture<T>,IFuture<T>>(DoApply));
        }

        private void DoApply(IFuture<T> msg, IFuture<T> ans)
        {
            // Read value
            var msgT = msg.Result();
            // Send value
            ans.SendMessage(msgT);
        }

        public IFuture<T> Add(IFuture<T> msg)
        {
            var future = new Future<T>();
            SendMessage(msg);
            return future;
        }
    }
}
