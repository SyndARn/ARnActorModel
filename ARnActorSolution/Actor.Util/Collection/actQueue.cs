using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{

    public class actBehalf : actActor
    {
        public actBehalf() : base()
        {
            Become(new bhvBehavior<Tuple<Action,IActor>>(DoIt)) ;
        }

        private void DoIt(Tuple<Action, IActor> msg)
        {
            msg.Item2.SendMessage(msg.Item1);
            Become(null);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "msg")]
    public class msgQueue<T>
    {
        public bool Result;
        public T Data;
    }

    public class actQueue<T> : actAction<T>
    {
        private Queue<T> fQueue = new Queue<T>();
        
        public actQueue()
            : base()
        {
        }

        public void Queue(T at)
        {
            SendAction(DoQueue, at);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public async Task<msgQueue<T>> TryDequeue()
        {
            // new actBehalf().
            var retVal = Receive(t => { return t is msgQueue<T>; }) ;
            SendAction(DoDequeue);
            return await retVal as msgQueue<T>;
        }

        private void DoQueue(T at)
        {
            fQueue.Enqueue(at);
        }

        private void DoDequeue()
        {
            if (fQueue.Count > 0)
            {
                SendMessage(new msgQueue<T>() { Result = true, Data = fQueue.Dequeue()});
            }
            else
            {
                SendMessage(new msgQueue<T>() { Result = false, Data = default(T) });
            }
        }

    }

}
