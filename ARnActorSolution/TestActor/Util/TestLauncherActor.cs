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
    [Serializable]
    public class TestLauncherException : Exception
    {
        public TestLauncherException() : base()
        {
        }

        public TestLauncherException(string message) : base(message)
        {
        }

        public TestLauncherException(string message,Exception inner)
            : base(message,inner)
        { }
    }

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

        public void Fail()
        {
            SendMessage(false);
        }

        public async Task<bool> Wait()
        {
            return await Wait(DefaultWait).ConfigureAwait(false);
        }

        public async Task<bool> Wait(int ms)
        {
            var val = await Receive(t => t is bool, ms).ConfigureAwait(false);
            var inTime = val != null;
            return inTime && (bool)val;
        }

        public static void Test(Action action)
        {
            Test( null, action, DefaultWait);
        }

        public static void Test(Action action, int timeOutMS)
        {
            Test(null, action, timeOutMS);
        }

        public static void Test(TestContext testContext, Action action)
        {
            Test(testContext, action, DefaultWait);
        }

        public static void Test(TestContext testContext, Action action, int timeOutMS)
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
                        launcher.Fail();
                        if (testContext != null)
                        {
                            testContext.WriteLine(e.Message);
                            testContext.WriteLine(e.StackTrace);
                        }
                        throw;
                    }
                });
            Task<bool> testResult = launcher.Wait(timeOutMS);
            if (launcher.fLauncherException != null)
            {
                throw new TestLauncherException(launcher.fLauncherException.Message, launcher.fLauncherException);
            }
            var result = testResult.Result;
            Assert.IsTrue(result, "Test Time Out");
        }
    }
}
