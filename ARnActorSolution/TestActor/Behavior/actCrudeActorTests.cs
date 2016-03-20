using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestActor;

namespace Actor.Util.Tests
{
    [TestClass()]
    public class actCrudeActorTests
    {
        actTestLauncher fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new actTestLauncher();
        }

        [TestMethod()]
        public void actCrudeActorTest()
        {
            var act = new actCrudeActor<string>();
            Assert.IsNotNull(act);
        }

        [TestMethod()]
        public void SelectTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void InsertTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateTest()
        {
            Assert.Fail();
        }
    }
}