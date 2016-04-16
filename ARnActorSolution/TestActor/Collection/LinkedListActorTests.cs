using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace TestActor
{
    [TestClass()]
    public class LinkedListActorTests
    {
        [TestMethod()]
        public void LinkedListActorTest()
        {
            TestLauncherActor.Test(
                () =>
                {
                    var ll = new LinkedListActor<string>();
                    Assert.IsNotNull(ll);
                }
                );
        }

        [TestMethod()]
        public void LinkedListAddbehaviorTest()
        {
            TestLauncherActor.Test(
                () =>
                {
                    var ll = new LinkedListActor<string>();
                    Assert.IsNotNull(ll);

                    ll.SendMessage(LinkedListOperation.Add, "Test1");

                }
                );
        }

        [TestMethod()]
        public void bhvLinkedListFirstTest()
        {
            TestLauncherActor.Test(
                () =>
                {
                    var ll = new LinkedListActor<string>();
                    Assert.IsNotNull(ll);

                    ll.SendMessage(LinkedListOperation.Add, "Test1");
                    var future = new Future<LinkedListOperation, string>();

                    ll.SendMessage(LinkedListOperation.First, (IActor)future);
                    Assert.AreEqual("Test1", future.Result().Item2);
                }
                );
        }

        [TestMethod()]
        public void bhvLinkedListNextTest()
        {
            TestLauncherActor.Test(
                () =>
                {
                    var ll = new LinkedListActor<string>();
                    Assert.IsNotNull(ll);

                    ll.SendMessage(LinkedListOperation.Add, "Test1");
                    ll.SendMessage(LinkedListOperation.Add, "Test2");
                    var future = new Future<LinkedListOperation, string>();

                    ll.SendMessage(LinkedListOperation.First, (IActor)future);
                    Assert.AreEqual("Test1", future.Result().Item2);

                    ll.SendMessage(LinkedListOperation.First, (IActor)future);
                    Assert.AreEqual("Test2", future.Result().Item2);
                }
                );
        }

        [TestMethod()]
        public void LinkedListBehaviorsTest()
        {
            var bhvMany = new LinkedListBehaviors<string>();
            Assert.IsNotNull(bhvMany);
        }
    }
}