using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.DbService.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Actor.DbService.Core.Model.Tests
{
    [TestClass()]
    public class FuncterTests
    {
        [TestMethod]
        [DataRow("Un","1")]
        [DataRow("Deux", "1")]
        [DataRow("Quatre", "2")]
        [DataRow("abruti", "3")]
        public void SyllabeCountTest(string word, string syllabeCount)
        {
            var func = Functer.RimeFuncter;
            var folder = new DataFolder("Test");
            var fields = func(word, folder).ToList();
            Assert.AreEqual(5, fields.Count);
            var field = fields.First(f => f.Name == "Syllabe");
            Assert.AreEqual(syllabeCount, field.Value);
        }

        [TestMethod]
        [DataRow("Un", "un")]
        [DataRow("Deux", "deu")]
        [DataRow("Quatre", "quatre")]
        [DataRow("abruti", "abruti")]
        [DataRow("Rois", "roi")]
        public void RimeTest(string word, string syllabeCount)
        {
            var func = Functer.RimeFuncter;
            var folder = new DataFolder("Test");
            var fields = func(word, folder).ToList();
            Assert.AreEqual(5, fields.Count);
            var field = fields.First(f => f.Name == "Rime");
            Assert.AreEqual(syllabeCount, field.Value);
        }
    }
}