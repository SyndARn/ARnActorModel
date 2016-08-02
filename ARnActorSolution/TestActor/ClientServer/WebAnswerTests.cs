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
    public class WebAnswerTests
    {
        [TestMethod()]
        public void CastTest()
        {
            var webAnswer2 = WebAnswer.Cast(new Uri(@"http://localhost"), "Test Answer");
            Assert.AreEqual("Test Answer", webAnswer2.Answer);
            Assert.AreEqual("localhost", webAnswer2.Url.Host);
        }
    }
}