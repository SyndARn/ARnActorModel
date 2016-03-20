using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestActor;
using System.Globalization;

namespace Actor.Base.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "act")]
    [TestClass()]
    public class actDirectoryTests
    {
        actTestLauncher fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new actTestLauncher();
            new actDirectory();
        }

        class actTestActor : actActor
        {
            public actTestActor()
            {
                Become(new bhvBehavior<Tuple<IActor,IActor,string>>(DoIt));
            }

            private void DoIt(Tuple<IActor, IActor, string> message)
            {
                actDirectory.GetDirectory().Find(this, message.Item3);
                var task = Receive(ask => { return (ask is Tuple<actDirectory.DirectoryRequest, IActor>); });
                if ((task.Result as Tuple<actDirectory.DirectoryRequest, IActor>).Item2 == message.Item2)
                {
                    message.Item1.SendMessage(true);
                }
            }
        }

        [TestMethod()]
        public void actDirectoryTest()
        {
            fLauncher.SendAction(() =>
            {
                var act = new actTestActor();
                act.SendMessage(new Tuple<IActor,IActor,string>(fLauncher, actDirectory.GetDirectory(), "Directory"));
            });
            Assert.IsTrue(fLauncher.Wait());
        }

        [TestMethod()]
        public void GetDirectoryTest()
        {
            Assert.IsTrue(actDirectory.GetDirectory() is actDirectory);
        }

        [TestMethod()]
        public void StatTest()
        {
            Assert.IsTrue(actDirectory.GetDirectory().Stat().StartsWith("Directory entries "));
        }

        [TestMethod()]
        public void DiscoTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RegisterTest()
        {
            fLauncher.SendAction(() =>
            {
                var act = new actActor();
                actDirectory.GetDirectory().Register(act, act.Tag.Id.ToString(CultureInfo.InvariantCulture));
                act.SendMessage(new Tuple<IActor, IActor, string>(fLauncher, act, act.Tag.Id.ToString(CultureInfo.InvariantCulture)));

            });
            Assert.IsTrue(fLauncher.Wait());
        }

        [TestMethod()]
        public void FindTest()
        {
            Assert.Fail();
        }
    }
}