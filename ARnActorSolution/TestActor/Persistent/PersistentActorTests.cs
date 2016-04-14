using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestActor
{
    [TestClass()]
    public class PersistentActorTests
    {

        TestLauncherActor fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new TestLauncherActor();
        }

        [TestMethod()]
        public void PersistentActorTest()
        {
            fLauncher.SendAction(() =>
            {
                var service = new MemoizePersistentService<string>();
                var persistent = new PersistentActor<string>(service, "TestActor");
                persistent.SendMessage("A");
                persistent.SendMessage("B");
                persistent.SendMessage("C");
                Assert.AreEqual("C", persistent.GetCurrent().Result());
                var persistent2 = new PersistentActor<string>(service, "TestActor");
                persistent2.Reload();
                Assert.AreEqual("C", persistent2.GetCurrent().Result());
                fLauncher.Finish();
            });
            Assert.IsTrue(fLauncher.Wait());
        }

        [TestMethod()]
        public void GetCurrentTest()
        {
            Assert.Fail();
        }
    }
}