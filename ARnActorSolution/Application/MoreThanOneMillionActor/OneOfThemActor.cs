using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Util;

namespace MoreThanOneMillionActor
{

    public class Glue : BaseActor
    {
       private  ReduceActor fReduce;
       StringObserver obs = new StringObserver();
       public Glue(int i, ReduceActor reduce)
        {
            fReduce = reduce;
            obs.RegisterObserver(fReduce);
            Become(new Behavior<int>(Go));
            SendMessage(i);
        }

        private void Go(int message)
        {
            List<IActor> fList = new List<IActor>();
            for(int i = 0; i< message;i++)
            {
                fList.Add(new OneOfThemActor(i));
            }
            foreach(var item in fList)
            {
                item.SendMessage(obs);
            }
        }
    }

    public class OneOfThemActor : StateFullActor<int>
    {
        public OneOfThemActor(int i) : base()
        {
            Set(i);
            AddBehavior(new Behavior<IActor>(Observe));
            // SendMessage(observer);
        }

        private void Observe(IActor observer)
        {
            observer.SendMessage(Get()) ;
        }
    }

    public class StringObserver : ObservableActor<int>
    {
        public StringObserver() : base()
        {

        }
    }


    public class ReduceActor : BaseActor
    {
        private int fSum  ;
        private int fCount;
        private ReceiveActor<string> fWait;
        public ReduceActor(int qtt) : base()
        {
            fSum = 0;
            fWait = new ReceiveActor<string>();
            Become(new Behavior<int>(t =>
            {
                fSum += t;
                fCount++;
                if (fCount >= qtt)
                {
                    fWait.SendMessage(new Tuple<IActor,string>(this,"Finish")) ;
                }
            }));
        }

        public int  WaitForResult()
        {
            var res = fWait.Wait(this, "Start").Result;
            return fSum;
        }

    }

}
