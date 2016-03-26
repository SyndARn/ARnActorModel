using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base ;
using Actor.Util ;
using Actor.Service;
using System.Diagnostics;

namespace Actor.TestApplication
{
    class ActorMain : BaseActor
    {
        private actCollection<string> collect;
        public ActorMain() : base()
        {
            Become(new bhvBehavior<string>(t => { return true; }, DoBehavior));
            new ActorService();
            SendMessage("Start");
        }

        private List<IActor> clientList;

        private void DoBehavior(string msg)
        {
            Console.WriteLine("Serv Start");
            var start = DateTime.UtcNow.Ticks;
            collect = new actCollection<string>();
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

            var actForeach = new BaseActor(new bhvForEach<string>());
            actForeach.SendMessage(new Tuple<IEnumerable<string>, Action<String>>(list,
                t => Console.WriteLine("list " + t)));


            Console.WriteLine("Should have work");

            var linkedlist = new actLinkedList<string>();
            for (int i = 0; i < 100; i++)
            {
                linkedlist.SendMessage(Tuple.Create(bhvLinkedListOperation.Add, i.ToString()));
            }

            new actEchoActor<Tuple<bhvLinkedListOperation, string>>(linkedlist, Tuple.Create(bhvLinkedListOperation.First, "5"));

            new actRing(1000,1000); // 10 sec

            new actLinkedList<string>();

            IActor aServer = new actEchoServer();
            clientList = new List<IActor>();
            for (int i = 0; i < 100; i++)
            {
                actEchoClient aClient = new actEchoClient();// new actEchoClient(aServer);
                // DirectoryRequest.SendRegister("client + " + i.ToString(), aClient);
                aClient.Connect("EchoServer");
                aClient.SendMessage("client-" + i.ToString());
                clientList.Add(aClient);
                // aClient.Disconnect();
            }
            var end = DateTime.UtcNow.Ticks;
            Console.WriteLine("All client allocated {0}", (double)(end - start) / 10000.0);

            // basic redirection
            IActor target = new BaseActor(new bhvBehavior<string>(t => { Console.WriteLine(t); }));
            IActor middle = new BaseActor(new bhvBehavior<string>(t => { t = t + " augmenté"; }));
            ((BaseActor)middle).RedirectTo(target);
            middle.SendMessage("Bonjour");


        }
   }

}
