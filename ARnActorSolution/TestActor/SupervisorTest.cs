using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;

namespace TestActor
{
    [TestClass]
    public class SupervisorTest
    {

        actTestLauncher fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new actTestLauncher();
        }

        [TestMethod]
        public void SupervisedTest()
        {
            fLauncher.SendAction(() =>
                {
                    var supervisor = new actSupervisor();
                    ISupervisedActor supervised = new actSupervisedActor();
                    supervisor.SendMessage(Tuple.Create(SupervisorAction.Register, supervised));
                    supervisor.SendMessage(Tuple.Create(SupervisorAction.Kill, supervised));
                    supervisor.SendMessage(Tuple.Create(SupervisorAction.Respawn, supervised));
                    fLauncher.Finish();
                });
            fLauncher.Wait();
        }
    }
}
