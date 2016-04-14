using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Service.Tests
{
    [TestClass()]
    public class MemoizePersistentServiceTests
    {
        [TestMethod()]
        public void WriteTest()
        {
            var service = new MemoizePersistentService<string>();
            Assert.IsNotNull(service);
            service.Write("A");
            service.Write("B");
            service.Write("C");
            var someString = service.Load();
            Assert.IsNotNull(someString);
            Assert.AreEqual(3, someString.Count());
            Assert.AreEqual("C", someString.Last());
        }

    }
}