using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util.ProducerConsumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestActor;

namespace Actor.Util.ProducerConsumer.Tests
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
                chain.SendMessage("start");
                fLauncher.Wait(10000);
                fLauncher.Finish();
            });
            fLauncher.Wait(11000);
        }

    }
}