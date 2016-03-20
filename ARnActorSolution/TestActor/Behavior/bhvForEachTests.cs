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

namespace Actor.Util.Tests
{
    [TestClass()]
    public class bhvForEachTests
    {
        actTestLauncher fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new actTestLauncher();
        }

        [TestMethod()]
        public void bhvForEachTest()
        {
            ConcurrentBag<string> start = new ConcurrentBag<string>();
            ConcurrentBag<string> end = new ConcurrentBag<string>();
            for (int i = 0; i < 100; i++)
                start.Add(i.ToString()) ;
            var actForeach = new actActor(new bhvForEach<string>());
            actForeach.SendMessage(new Tuple<IEnumerable<string>, Action<String>>(start,
                t => end.Add(t)));
            fLauncher.Wait(2000);
            foreach(var item in start)
            {
                Assert.IsTrue(end.Contains(item));
            }
        }
    }
}