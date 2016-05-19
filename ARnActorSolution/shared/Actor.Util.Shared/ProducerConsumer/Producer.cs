using Actor.Base;
using System;
using System.Collections.Generic;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Valider les arguments de méthodes publiques", MessageId = "0")]
        protected void DoIt(IActor anActor)
        {
            CheckArg.Actor(anActor);
            anActor.SendMessage(this);
        }

    }

    public class Consumer<T> : FsmActor<string, Work<T>>
    {
        public Consumer() : base("SleepState", null)
        {
            AddBehavior(new FsmBehavior<string, Work<T>>("SleepState", "BusyState", t =>
            {
                t.SendMessage(this);
            }, t => true));

            AddBehavior(new FsmBehavior<string, Work<T>>("BusyState", "SleepState", t =>
            {
                Buffer.SendMessage(this);
            }, t => true));

            

            AddBehavior(new Behavior<Work<T>>(
            t =>
            {
                SendMessage(new Tuple<string, Work<T>>(InternalCurrentState, t));
            }));
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

        public Buffer(IEnumerable<Consumer<T>> someConsumers) : base("BufferEmpty", null)
        {
            foreach (var item in someConsumers)
            {
                ConsList.Enqueue(item);
                item.Buffer = this;
            }

            AddBehavior(new Behavior<Work<T>>( t =>
            {
                SendMessage(new Tuple<string, Work<T>>(this.InternalCurrentState, t));
            }
                )) ;

            AddBehavior(new FsmBehavior<string, Work<T>>("BufferEmpty", "BufferNotEmpty",
                t =>
                {
                    if (ConsList.Count != 0)
                    {
                        var cons = ConsList.Dequeue();
                        cons.SendMessage(t);
                    }
                    else
                        WorkList.Enqueue(t);
                }, t => true ));

            AddBehavior(new FsmBehavior<string, Work<T>>("BufferNotEmpty", "BufferNotEmpty",
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
                t => WorkList.Count != 0));

            AddBehavior(new Behavior<Consumer<T>>(t =>
            {
                if (WorkList.Count == 0)
                {
                    ConsList.Enqueue(t);
                    InternalCurrentState = "BufferEmpty";
                }
                else
                {
                    t.SendMessage(WorkList.Dequeue());
                }
            }));
        }
    }




    public class Chain : BaseActor
    {
        public Chain() : base()
        {
            Become(new Behavior<Tuple<int,int,int>>(Start));
        }

        private void Start(Tuple<int, int, int> msg)
        {
            List<Consumer<long>> list = new List<Consumer<long>>();

            for (int i = 0; i < msg.Item2; i++)
                list.Add(new Consumer<long>());

            Buffer<long> buffer = new Buffer<long>(list);

            List<Producer<long>> list2 = new List<Producer<long>>();

            for (int i = 0; i < msg.Item1; i++)
                list2.Add(new Producer<long>(buffer));

            for(long i = 0; i<= msg.Item3;i++)
            {
                foreach (var prod in list2)
                    prod.SendMessage(i);
            }

            while(true)
            {
                var fut = buffer.GetCurrentState().Result(10000);
                if (fut == null) 
                        Console.WriteLine("Stop");
                if (fut != null ? fut.Item2 == "BufferEmpty" : false)
                    break;
            }

            Console.WriteLine("End of chain");

        }

    }


}


