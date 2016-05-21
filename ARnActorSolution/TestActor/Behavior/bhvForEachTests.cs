using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestActor;
using Actor.Base;
using System.Collections.Concurrent;
using System.Globalization;

namespace Actor.Util.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "bhv")]
    [TestClass()]
    public class bhvForEachTests
    {
        TestLauncherActor fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new TestLauncherActor();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "bhv")]
        [TestMethod()]
        public void bhvForEachTest()
        {
            ConcurrentBag<string> start = new ConcurrentBag<string>();
            ConcurrentBag<string> end = new ConcurrentBag<string>();
            for (int i = 0; i < 100; i++)
                start.Add(i.ToString(CultureInfo.InvariantCulture)) ;
            var actForeach = new BaseActor(new ForEachBehavior<string>());
            actForeach.SendMessage(new Tuple<IEnumerable<string>, Action<String>>(start,t => end.Add(t)));
            fLauncher.Wait(2000);
            foreach(var item in start)
            {
                Assert.IsTrue(end.Contains(item));
            }
        }
    }
}