using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Util;

namespace TestActor
{
    [TestClass()]
    public class bhvGraphTests
    {
        TestLauncherActor fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new TestLauncherActor();
        }


        class Receiver<T> : BaseActor
        {
            public T Call()
            {
                var r = Receive(t => t is T);

                return (T)r.Result;

            }
        }

        [TestMethod()]
        public void BehaviorGraphTest()
        {
            fLauncher.SendAction(() =>
            {
                var graph = new BehaviorGraph<string, int>();
                var nodeA = new NodeActor<string, int>();
                var nodeB = new NodeActor<string, int>();
                graph.AddNode(nodeA);
                graph.AddNode(nodeB);
                nodeA.AddEdge(nodeB);
                var act = new Receiver<Tuple<IActor, bool>>();
                nodeA.Adjacent(nodeB, act);
                Assert.IsTrue(act.Call().Item2);

                var nodeC = new NodeActor<string, int>();
                graph.AddNode(nodeC);
                nodeA.AddEdge(nodeC);

                var act2 = new Receiver<Tuple<IActor, IEnumerable<NodeActor<string, int>>>>();
                nodeA.Neighbors(act2);
                var r = act2.Call().Item2;
                Assert.IsNotNull(r);
                Assert.IsFalse(r.Contains(nodeA));
                Assert.IsTrue(r.Contains(nodeB));
                Assert.IsTrue(r.Contains(nodeC));

                // test as future
                var future = nodeA.Neighbors();
                Assert.IsNotNull(future);
                Assert.IsTrue(future.Result().Item2.Count() == 2);

                fLauncher.Finish();
            });
            Assert.IsTrue(fLauncher.Wait());
        }
    }
}