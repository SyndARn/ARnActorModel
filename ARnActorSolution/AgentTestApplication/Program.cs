using Actor.Util;
using ARnAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentTestApplication
{
    public class Program
    {

        static AgentWorld fWorld;
        static void Main(string[] args)
        {
            bool cont = true ;
            fWorld = new AgentWorld();
            var t = new TickerAgent();
            var m = new MarketAgent(t);
            for (var i = 0; i < 10000; i++ )
            {
                var ag = new TestAgent(i.ToString(),t,m);
            }
                while (cont)
                {
                    Console.WriteLine("S => Step 1 dt");
                    Console.WriteLine("O => Observer");
                    Console.WriteLine("Q => Quit");
                    string s = Console.ReadLine();
                    if (!string.IsNullOrEmpty(s))
                    {
                        switch (s.ToUpperInvariant())
                        {
                            case "S": Step(); break;
                            case "O": Observe(); break;
                            case "Q": cont = false; break;
                            default:
                                {
                                    new actEchoActor(new ActorAdminServer(), s);
                                    break;
                                }
                        }
                    }
                }
        }

        static void Step()
        {
            fWorld.Step();
        }

        static void Observe()
        {
            fWorld.Observe();
        }

    }
}
