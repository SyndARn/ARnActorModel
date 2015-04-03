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
            SendMessageTo(msg.Item1, msg.Item2);
            Become(null);
        }

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

        public async Task<Tuple<bool, T>> TryDequeue()
        {
            SendMessageTo(new Tuple<Action, IActor>(DoDequeue,this),new actBehalf()) ;
            var retVal = await Receive(t => { return t is Tuple<bool, T>; }) ;
            return retVal as Tuple<bool,T> ;
        }

        internal void DoQueue(T at)
        {
            fQueue.Enqueue(at);
        }

        private void DoDequeue()
        {
            if (fQueue.Count > 0)
            {
                SendMessageTo(new Tuple<bool, T>(true, fQueue.Dequeue()));
            }
            else
            {
                SendMessageTo(new Tuple<bool, T>(false, default(T))) ;
            }
        }

    }

}
