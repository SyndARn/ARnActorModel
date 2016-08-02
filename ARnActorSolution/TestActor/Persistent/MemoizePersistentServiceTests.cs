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
        class EventSourceString : EventSource<string>
        {

        }

        [TestMethod()]
        public void WriteTest()
        {
            var service = new MemoizePersistentService<string>();
            Assert.IsNotNull(service);
            var ev = new EventSourceString();
            ev.Apply("A");
            service.Write(ev);
            ev.Apply("B");
            service.Write(ev);
            ev.Apply("C");
            service.Write(ev);
            var someString = service.Load();
            Assert.IsNotNull(someString);
            Assert.AreEqual(3, someString.Count());
        }

    }
}