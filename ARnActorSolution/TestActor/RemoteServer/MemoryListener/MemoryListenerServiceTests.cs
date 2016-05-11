using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Server.Tests
{
    [TestClass()]
    public class MemoryListenerServiceTests
    {
        [TestMethod()]
        public void GetCommunicationContextTest()
        {
            var listenerService = new MemoryListenerService();
            Assert.IsTrue(listenerService is IListenerService);
            Assert.IsTrue(listenerService.GetCommunicationContext() is MemoryContextComm);
        }
    }
}