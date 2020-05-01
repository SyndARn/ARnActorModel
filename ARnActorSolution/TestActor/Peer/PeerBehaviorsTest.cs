using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using Actor.Base;
using System.Globalization;

namespace TestActor
{
    /// <summary>
    /// Description résumée pour PeerBehaviors
    /// </summary>
    [TestClass]
    public class PeerBehaviorsTest
    {
        public TestContext TestContext
        {
            get; set;
        }

        [TestMethod]
        public void PeerInterfaceTest()
        {
            TestLauncherActor.Test(TestContext, () =>
            {
                const int key = 10;
                const string value = "10";
                var peerActor = new PeerActor<int, string>();
                var finder = new Future<IPeerActor<int,string>>();

                peerActor.StoreNode(key, value);
                peerActor.FindPeer(key, finder);
                var result = finder.Result();
                Assert.IsTrue(result != null);
                var future = result.GetNode(key);
                result.DeleteNode(key);
                result.StoreNode(key, value);
                future = result.GetNode(key);
                Assert.AreEqual(value, future.Result());
            });
        }

        [TestMethod]
        public void PeerAgentTest()
        {
        }

        [TestMethod]
        public void CenterKeyTest()
        {
            var keyList = new List<string>();
            for(int i = 0;i<100;i++)
            {
                keyList.Add(string.Format(CultureInfo.InvariantCulture, "testKey{0}", i)) ;
            }
            var keyTest = CenterKey.Calc(keyList);
            Assert.AreEqual("testKey96", keyTest);
        }

        [TestMethod]
        public void PeerNewNodeTest()
        {
            TestLauncherActor.Test(TestContext, () =>
            {
                PeerActor<int, string> node1 = new PeerActor<int, string>();
                PeerActor<int, string> node2 = new PeerActor<int, string>();
                PeerActor<int, string> node3 = new PeerActor<int, string>();
                PeerActor<int, string> node4 = new PeerActor<int, string>();
                node1.NewPeer(node2, node2.GetHashKey().Result());
                node1.NewPeer(node3, node3.GetHashKey().Result());
                node1.NewPeer(node4, node4.GetHashKey().Result());
                node1.StoreNode(10, "10");
                node2.StoreNode(20, "20");
                node3.StoreNode(30, "30");
                var future = new Future<IPeerActor<int,string>>();
                node1.FindPeer(30, future);
                IPeerActor<int, string> result = future.Result();
                Assert.IsNotNull(result);
                // Assert.AreEqual(node3.Tag.Key(), result.Tag.Key());
                future = new Future<IPeerActor<int, string>>();
                node1.FindPeer(40, future);
                result = future.Result();
                Assert.IsNotNull(result);
                // Assert.AreEqual(node1.Tag.Key(), result.Tag.Key());
            });
        }
    }
}
