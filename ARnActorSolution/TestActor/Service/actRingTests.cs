using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;


namespace TestActor
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
                var future = new Future<string>();
                new RingActor(1000, 1000, future); // 10 sec

                var result = future.Result();

                Assert.IsFalse(string.IsNullOrEmpty(result));

                Assert.IsTrue(result.Contains("End Test"));

                Assert.IsTrue(result.Contains("Elapsed"));

                fLauncher.Finish();
            });
            fLauncher.Wait(20000);
        }
    }
}