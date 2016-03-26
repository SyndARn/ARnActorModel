using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;

namespace TestActor
{
    [TestClass]
    public class StateBehaviorTest
    {

        private static actTestLauncher fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new actTestLauncher();
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
