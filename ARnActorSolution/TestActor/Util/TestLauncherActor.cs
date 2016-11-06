using Actor.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestActor
{
    public class TestLauncherActor : ActionActor
    {
        public Exception ExceptionCatched { get; set; }

        public TestLauncherActor()
            : base()
        {
        }

        public void Finish()
        {
            SendMessage(true);
        }

        public bool Wait()
        {
            return Wait(10000);
        }

        public bool Wait(int ms)
        {
            var val = Receive(t => t is bool);
            var inTime = val.Wait(ms);
            return inTime && (bool)val.Result;
        }

        public static void Test(Action action)
        {
            Test(action, 20000);
        }

        public static void Test(Action action, int timeOutMS)
        {
            var launcher = new TestLauncherActor();
            launcher.SendAction(
                () =>
                {
                    try
                    {
                        action();
                        launcher.Finish();
                    }
                    catch (Exception e)
                    {
                        launcher.ExceptionCatched = e;
                    }
                });
            bool testResult = launcher.Wait(timeOutMS);
            if (launcher.ExceptionCatched != null)
            {
                throw launcher.ExceptionCatched;
            }
            Assert.IsTrue(testResult, "Test Time Out");
        }
    }
}
