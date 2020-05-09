using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.DbService.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;
using Actor.Util;
using System.Threading.Tasks;
using System.Linq;

namespace Actor.DbService.Core.Model.Tests
{
    [TestClass()]
    public class DataFolderTests
    {
        public TestContext TestContext { get; set; }


        private string source =

            @"
            Un
            Deux
            Trois
            ";

        [TestMethod()]
        public void ParseAndCountIndexTest()
        {
            TestLauncherActor.Test(() =>
            {
                var router = new IndexRouter();
                var folder = new DataFolder(source);
                folder.Parse(router, Functer.RimeFuncter);
                var query = new QueryByIndex("Word");
                var future = new Future<string, IEnumerable<Field>>();
                query.Launch(future, router);
                Task.Delay(5000).Wait();
                var result = future.Result();
                Assert.AreEqual(3, result.Item2.Count());
            });
        }

        [TestMethod()]
        public void LaunchWithReturnValueTest()
        {
            TestLauncherActor.Test(() =>
            {
                var router = new IndexRouter();
                var folder = new DataFolder(source);
                folder.Parse(router, Functer.RimeFuncter);
                var query = new QueryByIndex("Word");
                var results = query.Launch(router);
                Task.Delay(5000).Wait();
                foreach (var item in results)
                {
                    Assert.AreEqual("Word", item.FieldName);
                }
            });
        }
    }

    public class TestLauncherException : Exception
    {
        public TestLauncherException() : base()
        {
        }

        public TestLauncherException(string message) : base(message)
        {
        }

        public TestLauncherException(string message, Exception inner)
            : base(message, inner)
        { }

        protected TestLauncherException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }

    public class TestLauncherActor : ActionActor
    {
        public TestContext TestContext { get; set; }

        public const int DefaultWait = 30000;

        private Exception fLauncherException;

        public TestLauncherActor()
            : base()
        {
        }

        public void Finish() => SendMessage(true);

        public void Fail() => SendMessage(false);

        public Task<bool> Wait() => Wait(DefaultWait);

        public async Task<bool> Wait(int ms)
        {
            var val = await ReceiveAsync(t => t is bool, ms).ConfigureAwait(false);
            bool inTime = val != null;
            return inTime && (bool)val;
        }

        public static void Test(Action action) => Test(null, action, DefaultWait);

        public static void Test(Action action, int timeOutMS) => Test(null, action, timeOutMS);

        public static void Test(TestContext testContext, Action action)
        {
            Test(testContext, action, DefaultWait);
        }

        public static void Test(TestContext testContext, Action action, int timeOutMS)
        {
            var launcher = new TestLauncherActor();
            if (testContext == null)
            {
                testContext = launcher.TestContext;
            }

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
                        if (testContext != null)
                        {
                            testContext.WriteLine(e.Message);
                            testContext.WriteLine(e.StackTrace);
                        }
                        launcher.Fail();
                            // throw;
                        }
                });

            Task<bool> testResult = launcher.Wait(timeOutMS);
            if (launcher.fLauncherException != null)
            {
                throw new TestLauncherException(launcher.fLauncherException.Message, launcher.fLauncherException);
            }
            Assert.IsTrue(testResult.Result, "Test Time Out");
        }
    }

}