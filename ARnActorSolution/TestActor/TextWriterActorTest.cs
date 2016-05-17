using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using Actor.Util;
using System.IO;

namespace TestActor
{
    [TestClass]
    public class TextWriterActorTest
    {
        [TestMethod]
        public void TestTextWriter()
        {
            using (var memoryStream = new MemoryStream())
            {
                var textWriter = new TextWriterActor(memoryStream);
                TestLauncherActor.Test(() =>
                {
                    textWriter.SendMessage("1st line");
                    textWriter.SendMessage("2nd line");
                    textWriter.SendMessage("3rd line");
                });
                memoryStream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(memoryStream))
                {
                    Assert.AreEqual(reader.ReadLine(), "1st line");
                    Assert.AreEqual(reader.ReadLine(), "2nd line");
                    Assert.AreEqual(reader.ReadLine(), "3rd line");
                }
            }
        }
    }
}
