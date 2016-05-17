using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using Actor.Util;
using System.IO;
using System.Threading.Tasks;

namespace TestActor
{
    //[TestClass]
    //public class TextWriterActorTest
    //{
    //    [TestMethod]
    //    [Ignore]
    //    public void TestTextWriter()
    //    {
    //        TestLauncherActor.Test(() =>
    //        {
    //            var textWriter = new TextWriterActor("textwritertestfile.txt");
    //            textWriter.SendMessage("1st line");
    //            textWriter.SendMessage("2nd line");
    //            textWriter.SendMessage("3rd line");
    //            textWriter.Flush();
    //        });
    //        using (StreamReader reader = new StreamReader("textwritertestfile.txt"))
    //        {
    //            Assert.AreEqual(reader.ReadLine(), "1st line");
    //            Assert.AreEqual(reader.ReadLine(), "2nd line");
    //            Assert.AreEqual(reader.ReadLine(), "3rd line");
    //        }
    //    }
    //}
}
