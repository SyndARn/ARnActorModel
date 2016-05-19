﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Service
{
    public class TestResult
    {
        internal static DateTimeOffset start;
        internal static DateTimeOffset end;
        internal static TimeSpan Delta { get { return end.Subtract(start) ; } }
    }

    enum State { Start, Running }

    class RingNode : BaseActor
    {
        IActor fNextNode;
        int fTestRun = 0;
        public RingNode()
        {
            Become(new Behavior<Tuple<State,IActor>>(msg => 
            {
              return msg.Item1 == State.Start ;
            }, Behavior)) ;
        }

        private void Behavior(Tuple<State, IActor> msg)
        {
            fNextNode = msg.Item2;
            Become(
                new Behavior<ActorTag>(t =>
                { return (ActorTag)t != null; }, Running));
        }

        private void Running(ActorTag msg)
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

                if (fTestRun >= RingTest.fTest)
                {
                    TestResult.end = DateTimeOffset.UtcNow ;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("End Test " + fTestRun.ToString() + " " + TestResult.end.ToString());
                    sb.AppendLine("Elapsed " + fTestRun.ToString() + " " + TestResult.Delta.ToString()); // .ToString("N5"));
                    Console.WriteLine(sb.ToString());
                    if (RingTest.answer != null)
                    {
                        RingTest.answer.SendMessage(sb.ToString());
                    }
                }
            }
        }
    }

    public static class RingTest
    {
        internal static int fTest;
        internal static IActor answer;
    }

    public class RingActor : BaseActor
    {
        int fNode ;
        IActor firstNode = null;
        IActor lastNode;
        public RingActor(int aTest,int aNode, IActor answer = null)
        {
            RingTest.fTest = aTest;
            RingTest.answer = answer;
            fNode = aNode;
            IActor prevNode = null;

            IActor act;
            for (int i = 0; i < fNode; i++)
            {
                act = new RingNode();
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
            Become(new Behavior<bool>(msg => { return msg; }, Test));
            SendMessage(true);
        }

        protected void Test(bool msg)
        {
            TestResult.start = DateTimeOffset.UtcNow ;
            Console.WriteLine("Start at " + TestResult.start.ToString());
            for (int i = 0; i < RingTest.fTest; i++)
                firstNode.SendMessage(
                    new ActorTag(/*i*/));
        }
    }
}

