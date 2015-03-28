using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Util;
using Actor.RemoteLoading;
using Actor.MonteCarlo;
using Actor.Service;

namespace Actor.TestApplication
{
    class Program
    {
        static IActor fMain;
        static actMillion fMillion;
        static void Main(string[] args)
        {
            string lName = "";
            string lPort = "";
            if (args.Length > 0)
            {
                lName = args.Where(t => t.StartsWith("-n:")).FirstOrDefault();
                lPort = args.Where(t => t.StartsWith("-p:")).FirstOrDefault();
            }
            if (lName != "")
            {
                lName = lName.Replace("-n:", "");
            }
            else
                lName = "ARnActorServer";
            if (lPort != "")
            {
                lPort = lPort.Replace("-p:", "");
            }
            else
                lPort = "80";

            ActorServer.Start(lName, int.Parse(lPort));
            fMain = new ActorMain();

            // new actActionReceiver().ConsoleWrite("Welcome in an action world");

            //var Input = new SDRInput("Machine") ;
            //var Region = new HTMRegion(Input);

            string s = string.Empty;
            do
            {
                s = Console.ReadLine();
                if (string.IsNullOrEmpty(s) == false)
                switch (s)
                {
                    case "Many": { fMillion = new actMillion(); break; }
                    case "SendMany": { fMillion.Send(); break; }
                    case "quit": break;
                    case "Ring":
                        {
                            new actRing(10000, 10000); // 30 sec
                            break;
                        }
                    case "Rings":
                        {
                            Console.Write("Enter ring size : ");
                            var rs = Console.ReadLine();
                            Console.Write("Enter cycle : ") ;
                            var cy = Console.ReadLine();
                            int r =1 ;
                            int y = 1 ;
                            int.TryParse(rs, out r) ;
                            int.TryParse(cy, out y) ;
                            new actRing(y, r); // 30 sec
                            break;
                        }

                    case "Download":
                        {
                            Console.WriteLine("trying Download");
                            IActor down = new actActorDownloadTest();
                            break;
                        }
                    //case "Brain":
                    //    {
                    //        Console.Write("Enter word : ");
                    //        var s2 = Console.ReadLine();
                    //        Input.SetData(s2);
                    //        Region.Run();
                    //        Region.Print();
                    //        break;
                    //    }
                    //case "Learn":
                    //    {
                    //        Input.SetData("Hello world ! ") ;
                    //        for (int i = 0; i < 100; i++)
                    //        {
                    //            Region.Run();
                    //            Region.Print();
                    //        }
                    //        break;
                    //    }
                    case "Pricing" :
                        {
                            new actScheduler();
                            break;
                        }
                    case "ParserTest":
                        {
                            new ActorEchoActor(new ParserTest(), "");
                            break;
                        }
                    default:
                        {
                            new ActorEchoActor(new ActorAdminServer(), s);
                            break;
                        }
                }
            } while (s != "quit");
        }
    }
}
