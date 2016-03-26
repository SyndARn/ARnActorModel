using Actor.Base;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeSumNumber_Euler543
{
    class Program
    {
        static void Main(string[] args)
        {
            ActorServer.Start("localhost", 1123, false);
            var act = new Fibonacci();
            var res = act.Calc(10);
            Console.WriteLine(res);
            Console.ReadLine();
        }
    }
}
