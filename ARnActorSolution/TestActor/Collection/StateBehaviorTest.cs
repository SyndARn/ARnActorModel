using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;

namespace TestActor
{
    [TestClass]
    public class StateBehaviorTest
    {

        [TestMethod]
        public void ActorStateTest()
        {
            TestLauncherActor.Test(
                () =>
                {
                    var stateActor = new StateFullActor<string>();
                    string strTest = "Test actStateFullActor" ;
                    stateActor.Set(strTest);
                    Assert.AreEqual(strTest, stateActor.Get().Result());
                });
        }
    }
}
