using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{

    public interface IMsgQueue<T>
    {
        bool Result { get; }
        T Data { get; }
    }

    public class MsgQueue<T> : IMsgQueue<T>
    {
        public bool Result { get; }
        public T Data{ get; }
        public MsgQueue(bool aResult, T aData)
        {
            Result = aResult;
            Data = aData;
        }
}

    public class QueueActor<T> : ActionActor<T>
    {
        private Queue<T> fQueue = new Queue<T>();
        
        public QueueActor()
            : base()
        {
        }

        public void Queue(T at)
        {
            SendAction(DoQueue, at);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public async Task<IMsgQueue<T>> TryDequeue()
        {
            var retVal = Receive(t => { return t is IMsgQueue<T>; }) ;
            SendAction(DoDequeue);
            return await retVal as IMsgQueue<T>;
        }

        private void DoQueue(T at)
        {
            fQueue.Enqueue(at);
        }

        private void DoDequeue()
        {
            if (fQueue.Count > 0)
            {
                SendMessage(new MsgQueue<T>(true,fQueue.Dequeue()));
            }
            else
            {
                SendMessage(new MsgQueue<T>(false,default(T)));
            }
        }

    }

}
