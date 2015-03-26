using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Service
{
    class testResult
    {
        internal static DateTimeOffset start;
        internal static DateTimeOffset end;
        internal static TimeSpan Delta { get { return end.Subtract(start) ; } }
    }

    enum State { Start, Running }

    class actNode : actActor
    {
        IActor fNextNode;
        int fTestRun = 0;
        public actNode()
        {
            Become(new bhvBehavior<Tuple<State,IActor>>(msg => 
            {
              return msg.Item1 == State.Start ;
            }, Behavior)) ;
        }

        private void Behavior(Tuple<State, IActor> msg)
        {
            fNextNode = msg.Item2;
            Become(
                new bhvBehavior<actTag>(t =>
                { return (t is actTag); }, Running));
        }

        private void Running(actTag msg)
        {
            fTestRun++;
            if (fNextNode != null)
            {
                if ((fTestRun % 100) == 0)
                {
                    //var tr = new actTrace();
                    //tr.Start();
                    SendMessageTo(
                        msg, fNextNode);
                    //tr.Stop("End Node Ring");
                }
                else
                    SendMessageTo(
                        msg, fNextNode);
            }
            else
            {

                if (fTestRun >= actTest.fTest)
                {
                    testResult.end = DateTimeOffset.UtcNow ;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("End Test " + fTestRun.ToString() + " " + testResult.end.ToString());
                    sb.AppendLine("Elapsed " + fTestRun.ToString() + " " + testResult.Delta.ToString()); // .ToString("N5"));
                    Console.WriteLine(sb.ToString());
                    if (actTest.answer != null)
                    {
                        SendMessageTo(sb.ToString(), actTest.answer);
                    }
                }
            }
        }
    }

    public static class actTest
    {
        internal static int fTest;
        internal static IActor answer;
    }

    public class actRing : actActor
    {
        int fNode ;
        IActor firstNode = null;
        IActor lastNode;
        public actRing(int aTest,int aNode, IActor answer = null)
        {
            actTest.fTest = aTest;
            actTest.answer = answer;
            fNode = aNode;
            IActor prevNode = null;

            IActor act;
            for (int i = 0; i < fNode; i++)
            {
                act = new actNode();
                if (firstNode == null)
                    firstNode = act;
                else
                    SendMessageTo(
                        Tuple.Create(State.Start, act),prevNode);
                prevNode = act;
            }
            SendMessageTo(
                Tuple.Create(State.Start, (IActor)null),prevNode);
            lastNode = prevNode;
            Become(new bhvBehavior<Boolean>(msg => { return msg; }, Test));
            SendMessageTo(true,this);
        }

        protected void Test(Boolean msg)
        {
            testResult.start = DateTimeOffset.UtcNow ;
            Console.WriteLine("Start at " + testResult.start.ToString());
            for (int i = 0; i < actTest.fTest; i++)
                SendMessageTo(
                    new actTag(/*i*/),firstNode);
        }
    }
}

