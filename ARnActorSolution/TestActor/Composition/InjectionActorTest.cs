using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using Actor.Base;

namespace TestActor
{
    [TestClass]
    public class InjectionActorTest
    {
        internal class StringBehaviors : Behaviors
        {
            private string _data;

            public StringBehaviors()
            {
                BecomeBehavior(new Behavior<string>
                    (
                        s => true,
                        s => _data = s
                    ));
                AddBehavior(new Behavior<IFuture<string>>
                    (
                        (IFuture<string> f) => true,
                        (IFuture<string> f) => f.SendMessage(_data)
                    ));
            }
        }

        [TestMethod]
        public void BasicInjectionTest()
        {
            TestLauncherActor.Test(() =>
               {
                   var real = new StringBehaviors();
                   var inject = ActorInjection.Cast(real);
                   var actor = new BaseActor();
                   inject.Actor.SendMessage("Test Data");
                   IFuture<string> future = new Future<string>();
                   inject.Actor.SendMessage(future);
                   Assert.AreEqual("Test Data", future.Result());
               });
        }
    }
}
