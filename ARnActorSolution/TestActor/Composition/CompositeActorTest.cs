using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;

namespace TestActor
{
    internal class NumberActor : BaseActor
    {
        private readonly int Number;
        private readonly IActor Oper;

        public NumberActor(IActor oper,int number)
        {
            Number = number;
            Oper = oper;
            Become(new Behavior<string>(s => oper.SendMessage((IActor)this, Number)));
        }
    }

    internal class OperActor : BaseActor
    {
        public OperatorPlusBehavior trait { get; set; }

        public OperActor()
        {
            trait = new OperatorPlusBehavior();
            Become(trait);
        }
    }

    [TestClass]
    public class CompositeActorTest
    {
        [TestMethod]
        public void OperatorPlusTest()
        {
            TestLauncherActor.Test(() =>
            {
                var operPlus = new OperActor();
                var number1 = new NumberActor((IActor)operPlus,1);
                var number2 = new NumberActor((IActor)operPlus, 2);
                var output = new Future<int>();
                operPlus.trait.RegisterInput(number1);
                operPlus.trait.RegisterInput(number2);
                operPlus.trait.RegisterOutput(output);
                number1.SendMessage("start");
                number2.SendMessage("start");
                Assert.AreEqual(3, output.Result());
            });
        }
    }
}
