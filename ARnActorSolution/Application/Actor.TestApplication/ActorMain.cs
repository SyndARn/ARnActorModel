using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Server;
using Actor.Util;
using Actor.Service;
using System.Diagnostics;
using System.Dynamic;

namespace Actor.TestApplication
{
    class ActorMain : BaseActor
    {
        private CollectionActor<string> collect;
        public ActorMain() : base()
        {
            Become(new Behavior<string>(t => { return true; }, DoBehavior));
            new ActorService();
            SendMessage("Start");
        }

        private List<IActor> clientList;

        private void DoBehavior(string msg)
        {
            Console.WriteLine("Serv Start");
            var start = DateTime.UtcNow.Ticks;
            collect = new CollectionActor<string>();
            var list = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                collect.Add(i.ToString());
                list.Add(i.ToString());
            }

            foreach (var item in collect)
            {
                Console.WriteLine("Collect " + item);
            }

            var actForeach = new BaseActor(new ForEachBehavior<string>());
            actForeach.SendMessage(new Tuple<IEnumerable<string>, Action<String>>(list,
                t => Console.WriteLine("list " + t)));


            Console.WriteLine("Should have work");

            var linkedlist = new LinkedListActor<string>();
            for (int i = 0; i < 100; i++)
            {
                linkedlist.SendMessage(Tuple.Create(LinkedListOperation.Add, i.ToString()));
            }

            new EchoActor<Tuple<LinkedListOperation, string>>(linkedlist, Tuple.Create(LinkedListOperation.First, "5"));

            new RingActor(1000,1000); // 10 sec

            new LinkedListActor<string>();

            IActor aServer = new EchoServerActor();
            clientList = new List<IActor>();
            for (int i = 0; i < 100; i++)
            {
                EchoClientActor aClient = new EchoClientActor();// new actEchoClient(aServer);
                // DirectoryRequest.SendRegister("client + " + i.ToString(), aClient);
                aClient.Connect("EchoServer");
                aClient.SendMessage("client-" + i.ToString());
                clientList.Add(aClient);
                // aClient.Disconnect();
            }
            var end = DateTime.UtcNow.Ticks;
            Console.WriteLine("All client allocated {0}", (double)(end - start) / 10000.0);

            // basic redirection
            IActor target = new BaseActor(new Behavior<string>(t => { Console.WriteLine(t); }));
            IActor middle = new BaseActor(new Behavior<string>(t => { t = t + " augmenté"; }));
            ((BaseActor)middle).RedirectTo(target);
            middle.SendMessage("Bonjour");


            // dynamic use DLR to handle the send / dispatch business ...
            dynamic test = new ExpandoObject();
            dynamic dyn = new DynamicActor(test);
            test.PrintSomething = (Action<string>)((t) => Console.WriteLine(t));
            dyn.PrintSomething("Got it !");
        }
   }

}
