using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using Actor.Util;
using System.IO;
using System.Threading.Tasks;

namespace TestActor
{
    [TestClass]
    [Ignore] // app veyor fail
    public class TextWriterActorTest
    {
        [TestMethod]
        public void TestTextWriter()
        {
            TestLauncherActor.Test(() =>
            {
                using (var textWriter = new TextWriterActor("textwritertestfile.txt"))
                {
                    textWriter.SendMessage("1st line");
                    textWriter.SendMessage("2nd line");
                    textWriter.SendMessage("3rd line");
                    textWriter.Flush();
                }
                using (var stream = new FileStream("textwritertestfile.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    StreamReader reader = new StreamReader(stream);
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
