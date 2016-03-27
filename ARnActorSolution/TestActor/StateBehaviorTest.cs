using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;

namespace TestActor
{
    [TestClass]
    public class StateBehaviorTest
    {

        private static TestLauncherActor fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new TestLauncherActor();
        }

        [TestMethod]
        public void ActorStateTest()
        {
            fLauncher.SendAction(
                () =>
                {
                    var stateActor = new StateFullActor<string>();
                    string strTest = "Test actStateFullActor" ;
                    stateActor.Set(strTest);
                    Assert.AreEqual(strTest, stateActor.Get());
                });
        }
    }
}
