using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestActor;
using Actor.Base;
using System.IO;

namespace Actor.Service.Tests
{
    [TestClass()]
    public class TraceActorTests
    {

        public TestContext TestContext { get; set; }

        [TestMethod()]
        public void TraceActorTest()
        {
            TestLauncherActor.Test(() =>
            {
                var trace = new TraceActor();
                trace.Start();
                trace.Stop("Test End Trace");
                Task.Delay(1000).Wait();
            });
            var filename = Path.Combine(Environment.CurrentDirectory,"TraceLogger");
            List<string> strings = new List<string>();
            using (var stream = new StreamReader(filename))
            {
                while (!stream.EndOfStream)
                {
                    strings.Add(stream.ReadLine());
                }
            }
            Assert.IsTrue(strings.Any(t => t.Contains("Test End Trace")));
        }
    }
}