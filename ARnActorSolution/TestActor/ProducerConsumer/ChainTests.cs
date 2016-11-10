using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actor.Util;
using Actor.Base;

namespace TestActor
{
    [TestClass()]
    public class ChainTests
    {

        [TestMethod()]
        public void ChainTest()
        {
            TestLauncherActor.Test(() =>
                {
                    var chain = new Chain();
                    chain.SendMessage(4, 4, 4);
                });
        }

    }
}
