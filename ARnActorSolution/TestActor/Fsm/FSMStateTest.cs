using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using Actor.Base;

namespace TestActor.Fsm
{
    [TestClass]
    public class FSMStateTest
    {

        [TestMethod]
        public void FiniteStateMachineTest()
        {
            TestLauncherActor.Test(() =>
            {
                var behaviors = new List<FsmBehavior<string, int>>();
                behaviors.Add(new FsmBehavior<string, int>("StartState", "MidState", i => { }, i => i == 1));
                behaviors.Add(new FsmBehavior<string, int>("MidState", "EndState", i => { }, i => i == 2));
                var fsmActor = new FsmActor<string, int>("StartState", behaviors);

                var currentState1 = fsmActor.GetCurrentState();
                Assert.IsTrue(currentState1.Result() == "StartState");

                fsmActor.SendMessage("StartState", 1);

                var currentState2 = fsmActor.GetCurrentState();
                Assert.IsTrue(currentState2.Result() == "MidState");

                fsmActor.SendMessage("MidState", 2);

                var currentState3 = fsmActor.GetCurrentState();
                Assert.IsTrue(currentState3.Result() == "EndState");

                fsmActor.SendMessage("MidState", 3);

                // unchanged shoud be
                var currentState4 = fsmActor.GetCurrentState();
                Assert.IsTrue(currentState4.Result() == "EndState");
            });
        }
    }
}
