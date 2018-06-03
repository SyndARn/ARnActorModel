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
        public void NetDataContractSerializeTest()
        {
            IActor actor = new BaseActor();
            const string someData = "TestData";
            SerialObject serialObject = new SerialObject(someData, actor.Tag);

            Assert.AreEqual("TestData", serialObject.Data);
            Assert.AreEqual(actor.Tag, serialObject.Tag);

            NetDataContractSerializeService service = new NetDataContractSerializeService();
            using (MemoryStream serializeStream = new MemoryStream())
            {
                service.Serialize(serialObject.Data,serialObject.Tag, serializeStream);
                serializeStream.Seek(0, SeekOrigin.Begin); // rewind

                using (MemoryStream deserializeStream = new MemoryStream())
                {
                    serializeStream.CopyTo(deserializeStream);
                    SerialObject returnObject = service.Deserialize(deserializeStream) as SerialObject;
                    Assert.IsNotNull(returnObject);
                    Assert.IsNotNull(returnObject.Data);
                    Assert.IsNotNull(returnObject.Tag);
                    Assert.AreEqual("TestData", returnObject.Data);
                    Assert.AreEqual(actor.Tag, returnObject.Tag);
                }
            }
        }
    }

    [TestClass()]
    public class DataContractActorSerializeServiceTests
    {
        [TestMethod()]
        public void DataContractSerializeTest()
        {
            IActor actor = new BaseActor();
            const string someData = "TestData";
            DataContractObject serialObject = new DataContractObject(someData, actor.Tag);

            Assert.AreEqual("TestData", serialObject.Data);
            Assert.AreEqual(actor.Tag, serialObject.Tag);

            DataContractSerializeService service = new DataContractSerializeService();
            using (MemoryStream serializeStream = new MemoryStream())
            {
                service.Serialize(serialObject.Data, serialObject.Tag, serializeStream);
                serializeStream.Seek(0, SeekOrigin.Begin); // rewind

                using (MemoryStream deserializeStream = new MemoryStream())
                {
                    serializeStream.CopyTo(deserializeStream);
                    deserializeStream.Seek(0, SeekOrigin.Begin);
                    DataContractObject returnObject = service.Deserialize(deserializeStream) as DataContractObject ;
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