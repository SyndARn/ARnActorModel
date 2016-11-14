using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Actor.Server;
using Actor.Util;

namespace TestActor
{
    [TestClass()]
    public class DirectoryActorTests
    {
        TestLauncherActor fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new TestLauncherActor();
            var act = new DirectoryActor();
            Assert.IsNotNull(act, "Can't create Directory Actor");
        }

        internal class DirectoryTestActor : BaseActor
        {
            public DirectoryTestActor()
            {
                Become(new Behavior<IActor,IActor,string>(DoIt));
            }

            private void DoIt(IActor caller, IActor lookup, string name)
            {
                DirectoryActor.GetDirectory().Find(this, name);
                var task = Receive(ask => { return (ask is IMessageParam<DirectoryActor.DirectoryRequest, IActor>); });
                if ((task.Result as IMessageParam<DirectoryActor.DirectoryRequest, IActor>).Item2 == lookup)
                {
                    caller.SendMessage(true);
                }
            }
        }

        [TestMethod()]
        public void DirectoryFindActorTest()
        {
            TestLauncherActor.Test(() =>
            {
                var dirtest = new BaseActor();
                DirectoryActor.GetDirectory().Register(dirtest, dirtest.Tag.Key());
                var result = DirectoryActor.GetDirectory().FindActor(dirtest.Tag.Key()).Result(5000);
                Assert.IsTrue(result.Item1 == DirectoryActor.DirectoryRequest.Find);
                Assert.IsNotNull(result.Item2);
                Assert.AreEqual(dirtest, result.Item2);
            },-1);
        }

        [TestMethod()]
        public void DirectoryActorTest()
        {
            fLauncher.SendAction(() =>
            {
                IActor act = new DirectoryTestActor();
                act.SendMessage(fLauncher, DirectoryActor.GetDirectory(), "Directory");
            });
            Assert.IsTrue(fLauncher.Wait(100000));
        }

        [TestMethod()]
        public void GetDirectoryTest()
        {
            Assert.IsTrue(DirectoryActor.GetDirectory() is DirectoryActor);
        }

        [TestMethod()]
        public void StatTest()
        {
            Assert.IsTrue(DirectoryActor.GetDirectory().Stat().StartsWith("Directory entries "));
        }

        internal class DiscoTestActor : BaseActor
        {
            private IActor fLauncher;

            public DiscoTestActor(IActor aLauncher)
            {
                fLauncher = aLauncher;
                Become(new Behavior<Dictionary<string, string>>(ReceiveDisco));
            }

            private void ReceiveDisco(Dictionary<string, string> msg)
            {
                Assert.IsNotNull(msg);
                fLauncher.SendMessage(true);
            }
        }

        [TestMethod()]
        public void DiscoTest()
        {
            ActorServer.Start("localhost", 80,null);
            fLauncher.SendAction(() =>
            {
                var act = new DiscoTestActor(fLauncher);
                DirectoryActor.GetDirectory().Disco(act);
                fLauncher.Finish();
            }
            );
            Assert.IsTrue(fLauncher.Wait());
        }

        [TestMethod()]
        public void RegisterTest()
        {
            fLauncher.SendAction(() =>
            {
                var act = new DirectoryTestActor();
                DirectoryActor.GetDirectory().Register(act, act.Tag.Key());
                act.SendMessage(fLauncher, act, act.Tag.Key());
                fLauncher.Finish();
            });
            Assert.IsTrue(fLauncher.Wait());
        }

        [TestMethod()]
        public void FindTest()
        {
            fLauncher.SendAction(() =>
                {
                var dirtest = new DirectoryTestActor();
                DirectoryActor.GetDirectory().Register(dirtest, dirtest.Tag.Key());
                    fLauncher.Finish();
                });
            Assert.IsTrue(fLauncher.Wait());
        }
    }
}