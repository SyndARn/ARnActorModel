using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using TestActor;

namespace Actor.Util.Test
{


    internal class SequenceActor : BaseActor
    {

        private string TestData;
        private readonly IActor Actor;

        public SequenceActor(IActor actor) : base()
        {
            Actor = actor;
            Call(Sequence1);
        }

        private void Call(Action action)
        {
            Become(new ActionBehavior());
            SendMessage((Action)action);
        }

        [Behavior]
        private void Sequence1()
        {
            Call(Sequence2);
        }


        [Behavior]
        private void Sequence2()
        {
            Call(Sequence3);

        }

        [Behavior]
        private void Sequence3()
        {
            TestData = "Test Done";
            Actor.SendMessage(TestData);
        }

    }

    [TestClass]
    public class SequencingActorTest
    {
        [TestMethod]
        public void Sequencing1Test()
        {
            TestLauncherActor.Test(
                () =>
                {
                    var future = new Future<string>();
                    var sequenceActor = new SequenceActor(future);
                    var result = future.Result();
                    Assert.AreEqual("Test Done", result);
                });
        }
    }
}
