using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace ARnActorHelloWorld
{
    class Program
    {

        class HelloWorld : BaseActor
        {
            public HelloWorld() : base()
            {
                Become(new Behavior<string>(t => Console.WriteLine(t))) ;
            }
        }

        static void Main(string[] args)
        {
            var act = new HelloWorld();
            act.SendMessage("Hello world!");
            Console.ReadLine();
        }
    }
}
