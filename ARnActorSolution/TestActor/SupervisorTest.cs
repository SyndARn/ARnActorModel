using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;

namespace TestActor
{
    [TestClass]
    public class SupervisorTest
    {

        TestLauncherActor fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new TestLauncherActor();
        }

        [TestMethod]
        public void SupervisedTest()
        {
            fLauncher.SendAction(() =>
                {
                    var supervisor = new SupervisorActor();
                    ISupervisedActor supervised = new SupervisedActor();
                    supervisor.SendMessage(Tuple.Create(SupervisorAction.Register, supervised));
                    supervisor.SendMessage(Tuple.Create(SupervisorAction.Kill, supervised));
                    supervisor.SendMessage(Tuple.Create(SupervisorAction.Respawn, supervised));
                    fLauncher.Finish();
                });
            fLauncher.Wait();
        }
    }
}
