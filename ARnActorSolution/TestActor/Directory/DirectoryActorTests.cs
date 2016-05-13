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
                Become(new Behavior<Tuple<IActor,IActor,string>>(DoIt));
            }

            private void DoIt(Tuple<IActor, IActor, string> message)
            {
                DirectoryActor.GetDirectory().Find(this, message.Item3);
                var task = Receive(ask => { return (ask is Tuple<DirectoryActor.DirectoryRequest, IActor>); });
                if ((task.Result as Tuple<DirectoryActor.DirectoryRequest, IActor>).Item2 == message.Item2)
                {
                    message.Item1.SendMessage(true);
                }
            }
        }

        [TestMethod()]
        public void DirectoryActorTest()
        {
            fLauncher.SendAction(() =>
            {
                var act = new DirectoryTestActor();
                act.SendMessage(new Tuple<IActor,IActor,string>(fLauncher, DirectoryActor.GetDirectory(), "Directory"));
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
                DirectoryActor.GetDirectory().Register(act, act.Tag.Id.ToString());
                act.SendMessage(fLauncher, act, act.Tag.Id.ToString());
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
                DirectoryActor.GetDirectory().Register(dirtest, dirtest.Tag.Id.ToString());
                    fLauncher.Finish();
                });
            Assert.IsTrue(fLauncher.Wait());
        }
    }
}