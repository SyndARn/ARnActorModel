using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Service
{
    public class testResult
    {
        internal static DateTimeOffset start;
        internal static DateTimeOffset end;
        internal static TimeSpan Delta { get { return end.Subtract(start) ; } }
    }

    enum State { Start, Running }

    class actNode : BaseActor
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
                { return (actTag)t != null; }, Running));
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
                    fNextNode.SendMessage(msg);
                    //tr.Stop("End Node Ring");
                }
                else
                    fNextNode.SendMessage(msg);                
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
                        actTest.answer.SendMessage(sb.ToString());
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

    public class actRing : BaseActor
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
                    prevNode.SendMessage(
                        Tuple.Create(State.Start, act));
                prevNode = act;
            }
            prevNode.SendMessage(
                Tuple.Create(State.Start, (IActor)null));
            lastNode = prevNode;
            Become(new bhvBehavior<Boolean>(msg => { return msg; }, Test));
            SendMessage(true);
        }

        protected void Test(Boolean msg)
        {
            testResult.start = DateTimeOffset.UtcNow ;
            Console.WriteLine("Start at " + testResult.start.ToString());
            for (int i = 0; i < actTest.fTest; i++)
                firstNode.SendMessage(
                    new actTag(/*i*/));
        }
    }
}

