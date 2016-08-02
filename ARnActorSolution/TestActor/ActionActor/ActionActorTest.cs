using Actor.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Server;
using System.Collections.Concurrent;

namespace TestActor
{
    [TestClass]
    public class ActionActorTest
    {
        [TestMethod]
        public void TestActionActor()
        {
            TestLauncherActor.Test(
                () =>
                {
                    ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
                    var act = new ActionActor();
                    act.SendAction(() =>
                    {
                        var tst = new ActionActor<string>();
                        tst.SendAction((t) =>
                        {
                            queue.Enqueue(t);
                        }, "test1");
                        tst.SendAction((t) =>
                        {
                            queue.Enqueue(t);
                        }, "test2");
                        tst.SendAction((t) =>
                        {
                            queue.Enqueue(t);
                        }, "test3");
                        var future = new Future<int>();
                        tst.SendAction(() =>
                        {
                            future.SendMessage(queue.Count);
                        });
                        Assert.AreEqual(3, future.Result());
                    });
                }
                );
        }
    }
}
