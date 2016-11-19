using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestActor
{

    internal class TestStringPatternActor : StringPatternActor
    {
        protected override void DoApply(string msg)
        {
            if (msg == "Start")
            {

            }
            else
                if (msg == "End")
            {

            }
        }
    }


    [TestClass()]
    public class PatternActorTests
    {

        [TestMethod()]
        public void PatternActorTest()
        {
            TestLauncherActor.Test(
                () =>
                {
                    var act = new TestStringPatternActor();

                    act.SendMessage("Start");
                    act.SendMessage("End");

                });
        }
    }
}