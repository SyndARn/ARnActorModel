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
        [TestInitialize]
        public void Setup()
        {
            Functer.InitDico("CsvData\\wordSyllabe.csv", "CsvData\\wordRime.csv");
        }

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
            var field = fields.First(f => f.FieldName == "Syllabe");
            Assert.AreEqual(syllabeCount, field.Value);
        }

        [TestMethod]
        [DataRow("Un", "un")]
        [DataRow("Deux", "de")]
        [DataRow("Quatre", "quatre")]
        [DataRow("abruti", "outil")]
        [DataRow("Rois", "roi")]
        public void RimeTest(string word, string rime)
        {
            var func = Functer.RimeFuncter;
            var folder = new DataFolder("Test");
            var fields = func(word, folder).ToList();
            var field = fields.First(f => (f.FieldName == "Rime") && (f.Value == rime));
            Assert.AreEqual(rime, field.Value);
        }
    }
}