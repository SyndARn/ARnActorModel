using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.DbService.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace Actor.DbService.Core.Model.Tests
{
    [TestClass()]


    public class SyllableParserTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod()]
        public void IsVoyelTest()
        {
            foreach (char c in Enumerable.Range((int)'a', (int)'z').Select(i => (char)i))
            {
                var test = SyllableParser.IsVoyel(c);
                var expected = "a,e,i,o,u,y".Contains(c);
                Assert.AreEqual(expected, test,$"{c}");
            }
        }

        [TestMethod()]
        public void ParseAndCountSyllableTest()
        {
            var words = "Dieux=2,Triomphe=2,rencontrer=3,qu’aucun=2,haï=2,rebâtir=3,Et=1,Est=1,Bonjour=2,Trois=1,Vingt=1,difficile=3,facile=2,l'ouvrage=2";
            var split = words.Split(',');
            foreach (var wosyl in split)
            {
                var wosylsplit = wosyl.Split('=');
                var test = SyllableParser.Parse(wosylsplit[0]);
                Assert.AreEqual(wosylsplit[1], $"{test}", $"{wosylsplit[0]} {wosylsplit[1]} vs {test}");
            }
        }

        [TestMethod()]
        public void CheckReferenceDataTest()
        {
            var filename = "Model\\CsvData\\SyllabeTestData.txt";
            List<string> lines = new List<string>();
            using(var stream = new StreamReader(filename))
            {
                while (! stream.EndOfStream)
                {
                    lines.Add(stream.ReadLine());
                }
            }
            foreach(var l in lines)
            {
                var split = l.Split("=", 2);
                var test = SyllableParser.Parse(split[0]);
                Assert.AreEqual(split[1], $"{test}", $"{split[0]} found {test} expected {split[1]}");
            }
        }
    }
}