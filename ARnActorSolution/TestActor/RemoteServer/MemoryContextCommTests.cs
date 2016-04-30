using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Actor.Server.Tests
{
    [TestClass()]
    public class MemoryContextCommTests
    {

        [TestMethod()]
        public void SendReceiveStreamTest()
        {
            // what
            string testString = "SendReceiveStreamTest";

            // write
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine(testString);
            writer.Flush();

            // send
            MemoryContextComm comm = new MemoryContextComm();
            comm.SendStream("test", stream);

            // receive
            var streamReceived = comm.ReceiveStream();
            var reader = new StreamReader(streamReceived);
            string s = reader.ReadLine();

            // check
            Assert.AreEqual(testString, s);            

        }

    }
}