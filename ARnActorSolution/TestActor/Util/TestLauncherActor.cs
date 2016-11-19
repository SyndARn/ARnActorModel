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

        private Exception fLauncherException ;

        public TestLauncherActor()
            : base()
        {
        }

        public void Finish()
        {
            SendMessage(true);
        }

        public async Task<bool> Wait()
        {
            return await Wait(DefaultWait);
        }

        public async Task<bool> Wait(int ms)
        {
            var val = await Receive(t => t is bool, ms);
            var inTime = val != null;
            return inTime && (bool)val;
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
                        launcher.fLauncherException = e;
                        Debug.WriteLine(e.Message);
                        Debug.WriteLine(e.StackTrace);
                        throw ;
                    }
                });
            Task<bool> testResult = launcher.Wait(timeOutMS);
            if (launcher.fLauncherException != null)
            {
                throw new Exception(launcher.fLauncherException.Message, launcher.fLauncherException);
            }
            var result = testResult.Result;
            Assert.IsTrue(result, "Test Time Out");
        }
    }
}
