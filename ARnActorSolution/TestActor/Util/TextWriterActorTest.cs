using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using Actor.Util;
using System.IO;
using System.Threading.Tasks;
using System.Text;

namespace TestActor
{
    [TestClass]
    // [Ignore] // app veyor fail
    public class TextWriterActorTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestTextWriter()
        {
            var sb = new StringBuilder();
            sb.Append(TestContext.TestRunDirectory);
            sb.Append(@"\");
            sb.Append("testwritertestfile.txt");
            string fullPath = sb.ToString();
            TestLauncherActor.Test(() =>
            {
                using (var textWriter = new TextWriterActor(fullPath))
                {
                    textWriter.SendMessage("1st line");
                    textWriter.SendMessage("2nd line");
                    textWriter.SendMessage("3rd line");
                    textWriter.Flush();
                }

                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var reader = new StreamReader(stream);
                    try
                    {
                        Assert.AreEqual(reader.ReadLine(), "1st line");
                        Assert.AreEqual(reader.ReadLine(), "2nd line");
                        Assert.AreEqual(reader.ReadLine(), "3rd line");
                    }
                    finally
                    {
                        reader.Dispose();
                    }
                }
            });
        }
    }
}
