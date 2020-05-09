using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.DbService.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.DbService.Core.Model.Tests
{
    [TestClass()]
    public class FieldTests
    {
        [TestMethod()]
        public void FieldEqualsTest()
        {
            var field1 = new Field() { FieldName = "A", Keyword = "AK", Value = "AV", Uuid = "AU" };
            var field2 = new Field() { FieldName = "A", Keyword = "AK", Value = "AV", Uuid = "AU" };
            var field3 = new Field() { FieldName = "A", Keyword = "AK", Value = "AV", Uuid = "OT" };
            var field4 = new Field() { FieldName = "OT", Keyword = "AK", Value = "AV", Uuid = "AU" };
            Assert.AreEqual(field1, field2);
            Assert.AreNotEqual(field1, field3);
            Assert.AreNotEqual(field1, field4);
            Assert.AreNotEqual(field3, field4);
        }
    }
}