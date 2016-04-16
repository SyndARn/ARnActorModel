using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base ;

namespace TestActor
{
    [TestClass()]
    public class ObservableActorTests
    {

        TestLauncherActor fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new TestLauncherActor();
        }

        [TestMethod()]
        public void ObservableActorTest()
        {
            fLauncher.SendAction(() =>
                {
                    ObservableActor<string> act = new ObservableActor<string>();
                    Assert.IsNotNull(act);
                    Assert.IsTrue(act is ObservableActor<string>);
                    Assert.IsTrue(act is IActor);
                    fLauncher.Finish();
                }) ;
            Assert.IsTrue(fLauncher.Wait());
        }

        [TestMethod()]
        public void PublishDataTest()
        {
            fLauncher.SendAction(() =>
            {
                ObservableActor<string> act = new ObservableActor<string>();
                TestObserver observer1 = new TestObserver();
                act.RegisterObserver(observer1);
                TestObserver observer2 = new TestObserver();
                act.RegisterObserver(observer2);
                string testString = string.Format("Test {0}", observer1.Tag);
                act.SendMessage(testString);
                string result1 = observer1.GetData();
                Assert.AreEqual(result1, string.Format("Test {0}", observer1.Tag));
                string result2 = observer2.GetData();
                Assert.AreEqual(result1, string.Format("Test {0}", observer1.Tag));
                fLauncher.Finish();
            });
            Assert.IsTrue(fLauncher.Wait());
        }

        public class TestObserver : BaseActor
        {
            public TestObserver() : base()
            {
                Become(null);
            }

            public string GetData()
            {
                return Receive(t => t is string).Result as string;
            }

            public string GetDataInTime(int timeOutMs)
            {
                var r = Receive(t => t is string, timeOutMs).Result;
                if (r != null)
                    return r as string;
                else
                    return null;
            }
        }

        [TestMethod()]
        public void RegisterObserverTest()
        {
            fLauncher.SendAction(() =>
            {
                ObservableActor<string> act = new ObservableActor<string>();
                TestObserver observer = new TestObserver();
                act.RegisterObserver(observer);
                string testString = string.Format("Test {0}", observer.Tag);
                act.SendMessage(testString);
                string result = observer.GetData();
                Assert.AreEqual(result, string.Format("Test {0}", observer.Tag));
                fLauncher.Finish();
            });
            Assert.IsTrue(fLauncher.Wait());
        }

        [TestMethod()]
        public void UnRegisterObserverTest()
        {
            fLauncher.SendAction(() =>
            {
                ObservableActor<string> act = new ObservableActor<string>();
                TestObserver observer = new TestObserver();
                act.RegisterObserver(observer);
                string testString = string.Format("Test {0}", observer.Tag);
                act.SendMessage(testString);
                string result = observer.GetData();
                Assert.AreEqual(result, string.Format("Test {0}", observer.Tag));
                act.UnRegisterObserver(observer);
                act.SendMessage(testString);
                result = observer.GetDataInTime(1000);
                Assert.AreEqual(result, null);
                fLauncher.Finish();
            });
            Assert.IsTrue(fLauncher.Wait());
        }
    }
}