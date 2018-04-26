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
        public void TestActionActor2()
        {
            TestLauncherActor.Test(
                () =>
                {
                    ConcurrentDictionary<int,string> dico = new ConcurrentDictionary<int, string>();
                    var future = new Future<int, string>();
                    var act = new ActionActor();
                    act.SendAction(() =>
                    {
                        var tst = new ActionActor<int,string>();
                        tst.SendAction((i,t) => dico[i] = t, 1, "test1");
                        tst.SendAction((i, t) => dico[i] = t, 2, "test2");
                        tst.SendAction((i, t) => dico[i] = t, 3, "test3");
                        tst.SendAction(() =>
                        {
                            var s = string.Concat(dico.Values);
                            future.SendMessage(dico.Count,s);
                        });
                    });
                    var result = future.Result();
                    Assert.AreEqual(3, result.Item1);
                    Assert.AreEqual("test1test2test3", result.Item2);
                }
                );
        }

        [TestMethod]
        public void TestActionActor()
        {
            TestLauncherActor.Test(
                () =>
                {
                    ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
                    var act = new ActionActor();
                    var future = new Future<int>();
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
                        tst.SendAction(() =>
                        {
                            future.SendMessage(queue.Count);
                        });
                    });
                    Assert.AreEqual(3, future.Result());
                }
                );
        }
    }
}
