using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util.ProducerConsumer
{

    public class Work<T> : BaseActor
    {
        public Work()
        {
            Become(new Behavior<Tuple<IActor, T>>(DoIt));
        }

        protected void DoIt(Tuple<IActor, T> aMessage)
        {
            Console.WriteLine("I do work" + this.Tag.Id.ToString());
            aMessage.Item1.SendMessage(this);
        }

    }

    public class Consumer<T> : FsmActor<string, Work<T>>
    {
        public Consumer() : base("SleepState", null)
        {
            AddBehavior(new FsmBehavior<string, Work<T>>("SleepState", "BusyState", t =>
            {
                t.SendMessage(new Tuple<IActor, string>(this, "Start"));
            }, t => true));

            AddBehavior(new Behavior<Work<T>>(
            T =>
            {
                CurrentState = "SleepState";
                Buffer.SendMessage(this);
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
           var work = new Work<T>();
           Buffer.SendMessage(new Tuple<IActor, string, Work<T>>(this, "", work));
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

            AddBehavior(new FsmBehavior<string, Work<T>>("BufferEmpty", "BufferNotEmpty",
                t =>
                {
                    var cons = ConsList.Dequeue();
                    cons.SendMessage(t);
                }, t => WorkList.Count == 0));

            AddBehavior(new FsmBehavior<string, Work<T>>("BufferNotEmpty", "BufferNotEmpty",
                t =>
                {
                    if (ConsList.Count == 0)
                        WorkList.Enqueue(t);
                    else
                    {
                        var cons = ConsList.Dequeue();
                        cons.SendMessage(t);
                    }
                },
                t => WorkList.Count != 0));

            AddBehavior(new Behavior<Consumer<T>>(t =>
            {
                if (WorkList.Count == 0)
                {
                    ConsList.Enqueue(t);
                    CurrentState = "BufferEmpty";
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
            Become(new Behavior<String>(Start));
        }

        private void Start(string aMessage)
        {
            List<Consumer<long>> list = new List<Consumer<long>>();
            list.Add(new Consumer<long>());
            list.Add(new Consumer<long>());
            list.Add(new Consumer<long>());
            list.Add(new Consumer<long>());
            list.Add(new Consumer<long>());

            Buffer<long> buffer = new Buffer<long>(list);

            Producer<long> prod1 = new Producer<long>(buffer);
            Producer<long> prod2 = new Producer<long>(buffer);
            Producer<long> prod3 = new Producer<long>(buffer);
            Producer<long> prod4 = new Producer<long>(buffer);
            Producer<long> prod5 = new Producer<long>(buffer);
            for(long i = 0; i<= 1000;i++)
            {
                prod1.SendMessage(i);
                prod2.SendMessage(i);
                prod3.SendMessage(i);
                prod4.SendMessage(i);
                prod5.SendMessage(i);
            }

        }

    }


}


