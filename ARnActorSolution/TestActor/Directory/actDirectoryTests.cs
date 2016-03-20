using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestActor;

namespace Actor.Base.Tests
{
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
                Become(new bhvBehavior<IActor>(DoIt));
            }

            private void DoIt(IActor fLauncher)
            {
                actDirectory.GetDirectory().Find(this, "Directory");
                var task = Receive(ask => { return (ask is Tuple<actDirectory.DirectoryRequest, IActor>); });
                if ((task.Result as Tuple<actDirectory.DirectoryRequest, IActor>).Item2 is actDirectory)
                {
                    fLauncher.SendMessage(true);
                }
            }
        }

        [TestMethod()]
        public void actDirectoryTest()
        {
            var act = new actTestActor();
            act.SendMessage(fLauncher);
            fLauncher.Wait();
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
            Assert.Fail();
        }

        [TestMethod()]
        public void FindTest()
        {
            Assert.Fail();
        }
    }
}