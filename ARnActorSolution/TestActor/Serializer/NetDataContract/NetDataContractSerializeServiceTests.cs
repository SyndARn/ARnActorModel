using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.IO;

namespace Actor.Server.Tests
{
    [TestClass()]
    public class NetDataContractSerializeServiceTests
    {
        [TestMethod()]
        public void SerializeTest()
        {
            IActor actor = new BaseActor();
            string someData = "TestData";
            SerialObject serialObject = new SerialObject(someData, actor.Tag);

            Assert.AreEqual("TestData", serialObject.Data);
            Assert.AreEqual(actor.Tag, serialObject.Tag);

            NetDataContractSerializeService service = new NetDataContractSerializeService();
            using (MemoryStream serializeStream = new MemoryStream())
            {

                service.Serialize(serialObject, serializeStream);
                serializeStream.Seek(0, SeekOrigin.Begin); // rewind

                using (MemoryStream deserializeStream = new MemoryStream())
                {
                    serializeStream.CopyTo(deserializeStream);
                    var returnObject = service.DeSerialize(deserializeStream);
                    Assert.IsNotNull(returnObject);
                    Assert.IsNotNull(returnObject.Data);
                    Assert.IsNotNull(returnObject.Tag);
                    Assert.AreEqual("TestData", returnObject.Data);
                    Assert.AreEqual(actor.Tag, returnObject.Tag);
                }
            }

        }

    }
}