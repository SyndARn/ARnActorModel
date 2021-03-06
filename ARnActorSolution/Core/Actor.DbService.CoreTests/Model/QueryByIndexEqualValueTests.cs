﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.DbService.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;
using Actor.Util;
using System.Linq;
using System.Threading.Tasks;

namespace Actor.DbService.Core.Model.Tests
{
    [TestClass()]
    public class QueryByIndexEqualValueTests
    {
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Setup()
        {
            Functer.InitDico("CsvData\\wordSyllabe.csv", "CsvData\\wordRime.csv");
        }

        [TestMethod()]
        public void ReceiveAnswerTest()
        {
            const string source = @"Un Deux Trois Quatre";
            TestLauncherActor.Test(() =>
            {
                var indexRouter = new IndexRouter();
                var folder = new DataFolder(source);
                folder.Parse(indexRouter, Functer.RimeFuncter);
                var folder2 = new DataFolder("a on un en");
                folder2.Parse(indexRouter, Functer.RimeFuncter);

                Task.Delay(5000).Wait();

                var asker = new Future<string,IEnumerable<Field>>();
                var query = new QueryByIndexEqualValue("Rime", "roi");
                query.Launch(asker,indexRouter);
                var result = asker.Result();
                Assert.AreEqual(query.Uuid, result.Item1);
                Assert.AreEqual(folder.Uuid, result.Item2.First().Uuid);
            });
        }
    }
}