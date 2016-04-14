using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestActor
{
    [TestClass()]
    public class PatternActorTests
    {

        TestLauncherActor fLauncher;

        [TestInitialize]
        public void Setup()
        {
            fLauncher = new TestLauncherActor();
        }

        class TestStringPatternActor : StringPatternActor
        {
            protected override void DoApply(string msg)
            {
                // base.DoApply(msg);
                if (msg == "Start")
                {

                }
                else
                    if (msg == "End")
                {

                }
            }
        }

        [TestMethod()]
        public void PatternActorTest()
        {
            fLauncher.SendAction(
                () =>
                {
                    var act = new TestStringPatternActor();

                    act.SendMessage("Start");
                    act.SendMessage("End");

                    fLauncher.Finish();
                });
            Assert.IsTrue(fLauncher.Wait());
        }
    }
}