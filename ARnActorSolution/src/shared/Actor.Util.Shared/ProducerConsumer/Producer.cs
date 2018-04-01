using Actor.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{

    public class Work<T> : BaseActor
    {
        T fT;

        public Work(T aT)
        {
            fT = aT;
            Become(new Behavior<IActor>(DoIt));
        }

        protected void DoIt(IActor anActor)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(this);
        }

    }

    public class Consumer<T> : FsmActor<string, Work<T>>
    {
        public Consumer() : base()
        {
            var bhv = new FsmBehaviors<string, Work<T>>();

            bhv
                .AddRule("SleepState", null, t => t.SendMessage(this), "BusyState")
                .AddRule("BusyState", null, t => Buffer.SendMessage(this), "SleepState")
                .AddBehavior(new Behavior<Work<T>>(
                    t =>
                    {
                        this.SendMessage(GetCurrentState().Result(), t);
                    }));

            Become(bhv);
        }

        public Buffer<T> Buffer { get; set; }
    }

    public class Producer<T> : BaseActor
    {
        Buffer<T> Buffer;
        public Producer(Buffer<T> aBuffer) : base()
        {
            Buffer = aBuffer;
            Become(new Behavior<T>(DoProduce));
        }

        protected void DoProduce(T aT)
        {
           var work = new Work<T>(aT);
           Buffer.SendMessage(work);
        }
    }

    public class Buffer<T> : FsmActor<string, Work<T>>
    {
        Queue<Consumer<T>> ConsList = new Queue<Consumer<T>>();
        Queue<Work<T>> WorkList = new Queue<Work<T>>();

        public Buffer(IEnumerable<Consumer<T>> someConsumers) : base()
        {
            CheckArg.IEnumerable(someConsumers);
            foreach (var item in someConsumers)
            {
                ConsList.Enqueue(item);
                item.Buffer = this;
            }

            var bhv = new FsmBehaviors<string, Work<T>>();

            bhv
                .AddRule("BufferEmpty", null,
                t =>
                {
                    if (ConsList.Count != 0)
                    {
                        var cons = ConsList.Dequeue();
                        cons.SendMessage(t);
                    }
                    else
                        WorkList.Enqueue(t);
                }, "BufferNotEmpty")
                .AddRule("BufferNotEmpty", t => WorkList.Count != 0,
                t =>
                {
                    if (ConsList.Count != 0)
                    {
                        var cons = ConsList.Dequeue();
                        cons.SendMessage(t);
                    }
                    else
                        WorkList.Enqueue(t);
                },
                "BufferNotEmpty");
        }
    }
    
    public class Chain : BaseActor
    {
        public Chain() : base()
        {
            Become(new Behavior<int,int,int>(Start));
        }

        private void Start(int mi, int mj, int mk)
        {
            List<Consumer<long>> list = new List<Consumer<long>>();

            for (int i = 0; i < mj; i++)
                list.Add(new Consumer<long>());

            Buffer<long> buffer = new Buffer<long>(list);

            List<Producer<long>> list2 = new List<Producer<long>>();

            for (int i = 0; i < mi; i++)
                list2.Add(new Producer<long>(buffer));

            for(long i = 0; i<= mk;i++)
            {
                foreach (var prod in list2)
                    prod.SendMessage(i);
            }

            while(true)
            {
                var fut = buffer.GetCurrentState().Result(10000);
                if (fut == null) 
                        Debug.WriteLine("Stop");
                if (fut != null ? fut == "BufferEmpty" : false)
                    break;
            }

            Debug.WriteLine("End of chain");

        }

    }


}


