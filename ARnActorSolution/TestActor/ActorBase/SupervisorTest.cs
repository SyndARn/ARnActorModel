using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;

namespace TestActor
{
    [TestClass]
    public class SupervisorTest
    {


        [TestMethod]
        public void SupervisedTest()
        {
            TestLauncherActor.Test(() =>
                {
                    var supervisor = new SupervisorActor();
                    ISupervisedActor supervised = new SupervisedActor();
                    supervisor.SendMessage(SupervisorAction.Register, supervised);
                    supervisor.SendMessage(SupervisorAction.Kill, supervised);
                    supervisor.SendMessage(SupervisorAction.Respawn, supervised);
                });
        }
    }
}
