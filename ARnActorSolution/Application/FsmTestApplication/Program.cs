using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Server;
using Actor.Util;

namespace FsmTestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            ActorServer.Start("localhost", 80, null);
            Console.WriteLine("Starting FSM");

            BaseActor fsm = new Chain();
            fsm += Tuple.Create(1000, 4, 1000);

            Console.WriteLine("End of FSM");
            Console.ReadLine();
        }
    }
}
