using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreThanOneMillionActor
{
    class Program
    {
        static void Main(string[] args)
        {
            int qttActors = 1000000;
            Console.WriteLine("Glue for {0}", qttActors);
            var reduce = new ReduceActor(qttActors);
            var glue = new Glue(qttActors,reduce);
            Console.WriteLine("Result : {0}", reduce.WaitForResult());
            Console.ReadLine();
        }
    }
}
