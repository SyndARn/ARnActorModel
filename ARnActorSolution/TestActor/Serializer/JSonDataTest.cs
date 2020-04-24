using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Base;
using Actor.Util;
using System.Collections;

namespace TestActor
{
    [TestClass]
    public class JSonDataTest
    {
        private readonly string TestData =
            @"{""apiVersion"":""1.0"", ""data"":{ ""location"":""Paris, FRA"", ""temperature"":""54"", ""skytext"":""Sprinkles"", ""humidity"":""94"", ""wind"":""16"", ""date"":""2015-03-29"", ""day"":""Sunday"" } }: Receive {0}" ;

        [TestMethod]
        public void TestJSonParser()
        {
            Hashtable o = (Hashtable)Procurios.Public.JSON.JsonDecode(TestData);
            Assert.IsTrue(o != null);
            Assert.AreEqual(o["apiVersion"], "1.0");
        }



    }
}
