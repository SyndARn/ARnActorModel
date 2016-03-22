using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{

    public class MsgQueue<T>
    {
        public bool Result { get; }
        public T Data{ get; }
        public MsgQueue(bool aResult, T aData)
        {
            Result = aResult;
            Data = aData;
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

        public async Task<MsgQueue<T>> TryDequeue()
        {
            var retVal = Receive(t => { return t is MsgQueue<T>; }) ;
            SendAction(DoDequeue);
            return await retVal as MsgQueue<T>;
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
