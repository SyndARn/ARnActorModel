using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using Actor.Base;

namespace Actor.Server.Tests
{
    [TestClass()]
    public class DataContractObjectTests
    {
        [TestMethod()]
        public void DataContractObjectTest()
        {
            var dco = new DataContractObject();
            Assert.IsNull(dco.Data);
            Assert.IsNull(dco.Tag);
        }

        [TestMethod()]
        public void DataContractObjectTestWithData()
        {
            string someData = "someData";
            ActorTag tag = new ActorTag();
            var dco = new DataContractObject(someData,tag);
            Assert.AreEqual(someData, dco.Data);
            Assert.AreEqual(tag, dco.Tag);
        }
    }
}