using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using Actor.Base;

namespace TestActor
{
    [TestClass]
    public class FsmStateTest
    {
        [TestMethod]
        public void FSMBehaviorsTest()
        {
            TestLauncherActor.Test(() =>
            {
                var behaviors = new FsmBehaviors<string, int>();
                behaviors
                  .AddRule("StartState", i => i == 1, i => { }, "MidState")
                  .AddRule("MidState", i => i == 2, i => { }, "EndState");

                var fsmActor = new BaseActor(behaviors);

                var currentState1 = new Future<string>();
                fsmActor.SendMessage(currentState1);
                Assert.IsTrue(currentState1.Result() == "StartState");

                fsmActor.SendMessage(1);

                var currentState2 = new Future<string>();
                fsmActor.SendMessage(currentState2);
                Assert.IsTrue(currentState2.Result() == "MidState");

                fsmActor.SendMessage(2);

                var currentState3 = new Future<string>();
                fsmActor.SendMessage(currentState3);
                Assert.IsTrue(currentState3.Result() == "EndState");

                fsmActor.SendMessage(3);

                // unchanged shoud be
                var currentState4 = new Future<string>();
                fsmActor.SendMessage(currentState4);
                Assert.IsTrue(currentState4.Result() == "EndState");
            });
        }

        [TestMethod]
        public void FiniteStateMachineTest()
        {
            TestLauncherActor.Test(() =>
            {
                var behaviors = new FsmBehaviors<string, int>();
                behaviors
                  .AddRule("StartState",  i => i == 1, i => { },"MidState")
                  .AddRule("MidState",  i => i == 2, i => { },"EndState");
                var fsmActor = new FsmActor<string, int>(behaviors);

                var currentState1 = fsmActor.GetCurrentState();
                Assert.IsTrue(currentState1.Result() == "StartState");

                fsmActor.SendMessage(1);

                var currentState2 = fsmActor.GetCurrentState();
                Assert.IsTrue(currentState2.Result() == "MidState");

                fsmActor.SendMessage(2);

                var currentState3 = fsmActor.GetCurrentState();
                Assert.IsTrue(currentState3.Result() == "EndState");

                fsmActor.SendMessage(3);

                // unchanged shoud be
                var currentState4 = fsmActor.GetCurrentState();
                Assert.IsTrue(currentState4.Result() == "EndState");
            });
        }
    }
}
