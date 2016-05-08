using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestActor
{
    [TestClass()]
    public class RxObservableTests
    {
        [TestMethod()]
        public void RxObservableTest()
        {
            var rx = new RxObservable<string>();
            Assert.IsTrue(rx is IObservable<string>);
            Assert.IsTrue(rx is IActor);
        }

        class Observer : BaseActor, IObserver<string>
        {
            public Observer()
            {
                Become(new Behavior<string>(s =>
                {
                    Data = s;
                }));
                AddBehavior(new Behavior<IActor>(a =>
                {
                    a.SendMessage(Data);
                }));
            }

            public string GetResult()
            {
                var future = new Future<string>();
                SendMessage(future);
                return future.Result();
            }

            private string Data { get; set; }
            public void OnCompleted()
            {
                
            }

            public void OnError(Exception error)
            {
                
            }

            public void OnNext(string value)
            {
                Data = value;
            }
        }

        [TestMethod()]
        public void SubscribeTest()
        {
            TestLauncherActor.Test(() =>
            {
                var rx = new RxObservable<string>();
                var dsp = rx.Subscribe(new Observer());
                Assert.IsTrue(dsp is IDisposable);
            });
        }

        [TestMethod()]
        public void TrackTest()
        {
            TestLauncherActor.Test(() =>
            {
                var rx = new RxObservable<string>();
                var obs = new Observer();
                var dsp = rx.Subscribe(obs);
                rx.Track("Test Message");
                Assert.IsTrue(obs.GetResult() == "Test Message");
            });
        }
    }
}