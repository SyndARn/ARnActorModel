using System;
using Actor.Base;

namespace ConsoleCoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var actor1 = new BaseActor(new Behavior<string>(s => Console.WriteLine(s)));
            actor1.SendMessage("Hello World !");
            Console.ReadLine();
        }
    }
}