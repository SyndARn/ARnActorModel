using System;
using System.Linq;
using Actor.Base;
using Actor.MonteCarlo;
using Actor.RemoteLoading;
using Actor.Server;
using Actor.Service;
using Actor.Util;

namespace Actor.TestApplication
{
    class Program
    {
        static actMillion fMillion;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1806:Ne pas ignorer les résultats des méthodes", Justification = "<En attente>")]
        static void Main(string[] args)
        {
            string lName = "";
            string lPort = "";
            if (args.Length > 0)
            {
                lName = Array.Find(args, t => t.StartsWith("-n:", StringComparison.InvariantCulture));
                lPort = Array.Find(args, t => t.StartsWith("-p:",StringComparison.InvariantCulture));
            }
            if (!string.IsNullOrEmpty(lName))
            {
                lName = lName.Replace("-n:", "");
            }
            else
            {
                lName = "ARnActorServer";
            }

            if (!String.IsNullOrEmpty(lPort))
            {
                lPort = lPort.Replace("-p:", "");
            }
            else
            {
                lPort = "80";
            }

            // ActorServer.Start(lName, int.Parse(lPort), new HostRelayActor());
            ActorServer.Start(new ActorConfigManager());
            IActor fMain = new ActorMain();

            // new actActionReceiver().ConsoleWrite("Welcome in an action world");

            //var Input = new SDRInput("Machine") ;
            //var Region = new HTMRegion(Input);

            string s = string.Empty;
            do
            {
                s = Console.ReadLine();
                if (!string.IsNullOrEmpty(s))
                {
                    switch (s)
                    {
                        case "Many": { fMillion = new actMillion(); break; }
                        case "SendMany": { fMillion.Send(); break; }
                        case "quit": break;
                        case "Col":
                            {
                                var fLauncher = new TestLauncherActor();
                                fLauncher.SendAction(() =>
                                {
                                    var collect = new CollectionActor<string>();
                                    for (int i = 0; i < 100; i++)
                                    {
                                        collect.Add($"Test {i}");
                                    }

                                    if (collect.Count() != 100)
                                    {
                                        throw new Exception("failed");
                                    }
                                    // try to enum
                                    var enumerable = collect.ToList();
                                    if (enumerable.Count != 100)
                                        throw new ActorException("failed");
                                    // try a query
                                    var query = from col in collect
                                                where col.Contains('1')
                                                select col;
                                    if (query.Count() != 19)
                                        throw new ActorException("failed");
                                    fLauncher.Finish();
                                });
                                fLauncher.Wait();
                                break;
                            }
                        case "Ring":
                            {
                                new RingActor(10000, 10000); // 30 sec
                                break;
                            }
                        case "Rings":
                            {
                                Console.Write("Enter ring size : ");
                                var rs = Console.ReadLine();
                                Console.Write("Enter cycle : ");
                                var cy = Console.ReadLine();
                                int.TryParse(rs, out int r);
                                int.TryParse(cy, out int y);
                                new RingActor(y, r); // 30 sec
                                break;
                            }
                        case "Clients":
                            {
                                var start = DateTime.UtcNow.Ticks;
                                IActor aServer = new EchoServerActor();
                                for (int i = 0; i < 1000; i++)
                                {
                                    EchoClientActor aClient = new EchoClientActor();// new actEchoClient(aServer);
                                                                                    // DirectoryRequest.SendRegister("client + " + i.ToString(), aClient);
                                    aClient.Connect("EchoServer");
                                    aClient.SendMessage($"client-{i}");
                                    // aClient.Disconnect();
                                }
                                var end = DateTime.UtcNow.Ticks;
                                Console.WriteLine("All client allocated {0}", (double)(end - start) / 10000.0);
                                break;
                            }
                        case "Download":
                            {
                                Console.WriteLine("trying Download");
                                IActor down = new ActorDownloadTest();
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
                        case "Pricing":
                            {
                                new ActorScheduler();
                                break;
                            }
                        case "ParserTest":
                            {
                                EchoActor.Echo(new ParserTest(), "");
                                break;
                            }
                        default:
                            {
                                EchoActor.Echo(new ActorAdminServer(), s);
                                break;
                            }
                    }
                }
            } while (s != "quit");
        }
    }
}
