using Actor.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestActor
{
    public class TestLauncherActor : ActionActor
    {
        public const int DefaultWait = 30000;

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
            return Wait(DefaultWait);
        }

        public bool Wait(int ms)
        {
            var val = Receive(t => t is bool);
            var inTime = val.Wait(ms);
            return inTime && (bool)val.Result;
        }

        public static void Test(Action action)
        {
            Test(action, DefaultWait);
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
                        Debug.WriteLine(e.Message);
                        Debug.WriteLine(e.StackTrace);
                        throw ;
                    }
                });
                bool testResult = launcher.Wait(timeOutMS);
                Assert.IsTrue(testResult, "Test Time Out");
        }
    }
}
