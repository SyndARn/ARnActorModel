using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using Actor.Util;

namespace TestActor
{
    [TestClass]
    public class ReceiveActorTest
    {

        class TargetActor : BaseActor
        {
            public TargetActor()
            {
                Become(new Behavior<IActor, int>((a, i) =>
                 {
                     a.SendMessage(new Tuple<IActor,string>((IActor)this,i % 2 == 0 ? "even" : "odd"));
                 })) ;
            }
        }


        [TestMethod]
        public void ReceiveWaitTest()
        {
            TestLauncherActor.Test(() =>
            {
                var actor = new ReceiveActor<int, string>();
                var target = new TargetActor();
                var resultOdd = actor.Wait((IActor)target, 1);
                var resultEven = actor.Wait((IActor)target, 2);
                Assert.IsTrue(resultOdd.Result.Item2 == "Odd");
                Assert.IsTrue(resultEven.Result.Item2 == "Even");
            }) ;
        }
    }
}
