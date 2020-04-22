using System;
using System.Collections.Generic;
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
        static ActorMillion _million;

        static void Main(string[] args)
        {
            string lName = "";
            string lPort = "";
            if (args.Length > 0)
            {
                lName = Array.Find(args, t => t.StartsWith("-n:"));
                lPort = Array.Find(args, t => t.StartsWith("-p:"));
            }

            lName = !string.IsNullOrEmpty(lName) ? lName.Replace("-n:", "") : "ARnActorServer";

            if (!string.IsNullOrEmpty(lPort))
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
                        case "Many":
                            {
                                _million = new ActorMillion(); break;
                            }

                        case "SendMany":
                            {
                                _million.Send(); break;
                            }

                        case "quit": break;
                        case "Col":
                            {
                                var launcher = new TestLauncherActor();
                                launcher.SendAction(() =>
                                {
                                    var collect = new CollectionActor<string>();
                                    for (int i = 0; i < 100; i++)
                                    {
                                        collect.Add(string.Format("Test {0}", i));
                                    }

                                    if (collect.Count() != 100)
                                    {
                                        throw new Exception("failed");
                                    }
                                    // try to enum
                                    List<string> enumerable = collect.ToList();
                                    if (enumerable.Count != 100)
                                    {
                                        throw new ActorException("failed");
                                    }
                                    // try a query
                                    var query = from col in collect
                                                where col.Contains('1')
                                                select col;
                                    if (query.Count() != 19)
                                    {
                                        throw new ActorException("failed");
                                    }

                                    launcher.Finish();
                                });
                                launcher.Wait();
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
                                string rs = Console.ReadLine();
                                Console.Write("Enter cycle : ");
                                string cy = Console.ReadLine();
                                int.TryParse(rs, out int r);
                                int.TryParse(cy, out int y);
                                new RingActor(y, r); // 30 sec
                                break;
                            }
                        case "Clients":
                            {
                                long start = DateTime.UtcNow.Ticks;
                                IActor aServer = new EchoServerActor();
                                for (int i = 0; i < 1000; i++)
                                {
                                    var aClient = new EchoClientActor();// new actEchoClient(aServer);
                                                                                    // DirectoryRequest.SendRegister("client + " + i.ToString(), aClient);
                                    aClient.Connect("EchoServer");
                                    aClient.SendMessage("client-" + i.ToString());
                                    // aClient.Disconnect();
                                }

                                long end = DateTime.UtcNow.Ticks;
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
                                new EchoActor(new ParserTest(), "");
                                break;
                            }
                        default:
                            {
                                new EchoActor(new ActorAdminServer(), s);
                                break;
                            }
                    }
                }

            } while (s != "quit");
        }
    }
}
