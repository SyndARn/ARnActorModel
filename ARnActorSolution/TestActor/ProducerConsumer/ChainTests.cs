using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Util;
using Actor.Base;

namespace TestActor
{
    [TestClass()]
    public class ChainTests
    {
        TestLauncherActor fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new TestLauncherActor();
        }

        [TestMethod()]
        public void ChainTest()
        {
            fLauncher.SendAction(() =>
            {
                var chain = new Chain();
                chain.SendMessage(4,4,4);
                fLauncher.Finish();
            });
            Assert.IsTrue(fLauncher.Wait(11000));
        }

    }
}