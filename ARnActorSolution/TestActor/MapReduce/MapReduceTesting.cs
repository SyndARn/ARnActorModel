using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Actor.Util;
using Actor.Base;
using System.Linq;

namespace TestActor.MapReduce
{

    class MapReduceSimpleTest
    {
        public MapReduceSimpleTest()
        {

        }

        public void Go(string aFilename, IActor actor)
        {
            var mapReduce = new MapReduceActor<string, string, string, string, int>
                (
                // parse
                (a, d) =>
                {
                    for (int i = 0; i < 10; i++)
                        a.SendMessage(d,i.ToString()+" "+i.ToString());
                },
                // map
                (a, k, v) =>
                {
                    string[] stab = v.Split(' ');
                    foreach (var item in stab)
                    {
                        a.SendMessage(item, 1);
                    }

                },
                // reduce
                (k, v) =>
                {
                    int sum = v.Count();
                    return sum;
                },
                // output
                actor);
            mapReduce.SendMessage(aFilename);
        }
    }

    [TestClass]
    [Ignore]
    public class MapReduceTesting
    {

        [TestMethod]
        [Ignore]
        public void TestingSimpleMapReduce()
        {
            TestLauncherActor.Test(() =>
            {
                MapReduceSimpleTest mapReduce = new MapReduceSimpleTest();
                EnumerableActor<string> actor = new EnumerableActor<string>();
                mapReduce.Go("test map", actor);
                
                var result = actor.Count;
                Assert.IsTrue(result > 0);
            });
        }
    }
}
