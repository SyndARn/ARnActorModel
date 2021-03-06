﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Globalization;

namespace TestActor
{
    internal class TestObserver : BaseActor
    {
        public TestObserver() : base()
        {
        }

        public string GetData()
        {
            return ReceiveAsync(t => t is string).Result as string;
        }

        public string GetDataInTime(int timeOutMs)
        {
            var r = ReceiveAsync(t => t is string, timeOutMs).Result;
            if (r != null)
                return r as string;
            else
                return null;
        }
    }

    [TestClass()]
    public class ObservableActorTests
    {
        [TestMethod()]
        public void ObservableActorTest()
        {
            TestLauncherActor.Test(() =>
                {
                    ObservableActor<string> observableActor = new ObservableActor<string>();
                    Assert.IsNotNull(observableActor);
                    Assert.IsTrue(observableActor is ObservableActor<string>);
                    Assert.IsTrue(observableActor is IActor);
                });
        }

        [TestMethod()]
        public void PublishDataTest()
        {
            TestLauncherActor.Test(() =>
            {
                ObservableActor<string> observableActor = new ObservableActor<string>();
                TestObserver observer1 = new TestObserver();
                observableActor.RegisterObserver(observer1);
                TestObserver observer2 = new TestObserver();
                observableActor.RegisterObserver(observer2);
                string testString = $"Test {observer1.Tag}";
                observableActor.SendMessage(testString);
                string result1 = observer1.GetData();
                Assert.AreEqual(result1, $"Test {observer1.Tag}");
                string result2 = observer2.GetData();
                Assert.AreEqual(result2, $"Test {observer2.Tag}");
            });
        }

        [TestMethod()]
        public void RegisterObserverTest()
        {
            TestLauncherActor.Test(() =>
            {
                ObservableActor<string> act = new ObservableActor<string>();
                TestObserver observer = new TestObserver();
                act.RegisterObserver(observer);
                string testString = $"Test {observer.Tag}";
                act.SendMessage(testString);
                string result = observer.GetData();
                Assert.AreEqual(result, $"Test {observer.Tag}");
            });
        }

        [TestMethod()]
        public void UnRegisterObserverTest()
        {
            TestLauncherActor.Test(() =>
            {
                ObservableActor<string> act = new ObservableActor<string>();
                TestObserver observer = new TestObserver();
                act.RegisterObserver(observer);
                string testString = string.Format(CultureInfo.InvariantCulture, "Test {0}", observer.Tag);
                act.SendMessage(testString);
                string result = observer.GetData();
                Assert.AreEqual(result, string.Format(CultureInfo.InvariantCulture, "Test {0}", observer.Tag));
                act.UnregisterObserver(observer);
                act.SendMessage(testString);
                result = observer.GetDataInTime(1000);
                Assert.AreEqual(result, null);
            });
        }
    }
}