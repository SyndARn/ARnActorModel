using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Collections.Concurrent;
using System.Globalization;

namespace TestActor
{ 
    [TestClass()]
    public class ForEachTests
    {

        [TestMethod()]
        public void ForEachBehaviorTest()
        {
            TestLauncherActor.Test(() =>
            {
                ConcurrentBag<string> start = new ConcurrentBag<string>();
                ConcurrentBag<string> end = new ConcurrentBag<string>();
                for (int i = 0; i < 100; i++)
                {
                    start.Add(i.ToString(CultureInfo.InvariantCulture));
                }
                var actForeach = new BaseActor(new IBehavior[] { new ForEachBehavior<string>(), new MarkBehavior() } );
                actForeach.SendMessage((IEnumerable<string> )start, (Action<string>) end.Add);
                IFuture<BehaviorMark> future = new Future<BehaviorMark>();
                actForeach.SendMessage(BehaviorMark.Ask, future);
                var result = future.Result();
                Assert.IsNotNull(result);
                Assert.AreEqual(BehaviorMark.Reached, result);
                foreach (var item in start)
                {
                    Assert.IsTrue(end.Contains(item));
                }
            });
        }
    }
}