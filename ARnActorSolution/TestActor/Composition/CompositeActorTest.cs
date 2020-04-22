using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;

namespace TestActor
{
    internal class NumberActor : BaseActor
    {
        private readonly int _number;
        private readonly IActor _oper;

        public NumberActor(IActor oper,int number)
        {
            _number = number;
            _oper = oper;
            Become(new Behavior<string>(s => oper.SendMessage((IActor)this, _number)));
        }
    }

    internal class OperActor : BaseActor
    {
        public OperatorPlusBehavior Trait { get; set; }

        public OperActor()
        {
            Trait = new OperatorPlusBehavior();
            Become(Trait);
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
                operPlus.Trait.RegisterInput(number1);
                operPlus.Trait.RegisterInput(number2);
                operPlus.Trait.RegisterOutput(output);
                number1.SendMessage("start");
                number2.SendMessage("start");
                Assert.AreEqual(3, output.Result());
            });
        }
    }
}
