using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestActor;

namespace Actor.Service.Tests
{
    [TestClass()]
    public class actRingTests
    {
        TestLauncherActor fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new TestLauncherActor();
        }

        [TestMethod()]
        public void actRingTest()
        {
            fLauncher.SendAction(() =>
            {
                new actRing(1000, 1000); // 10 sec
                fLauncher.Finish();
            });
            fLauncher.Wait(10000);
        }
    }
}