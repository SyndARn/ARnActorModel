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
    class ActorMain : actActor
    {
        private actCollection<string> collect;
        public ActorMain() : base()
        {
            Become(new bhvBehavior<string>(t => { return true; }, DoBehavior));
            new ActorService();
            SendMessageTo("Start", this);
        }

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
            var actForeach = new actActor(new bhvForEach<string>());
            SendMessageTo(new Tuple<IEnumerable<string>, Action<String>>(list,
                t => Console.WriteLine("list " + t)),actForeach);

            foreach (var item in collect)
            {
                Console.WriteLine("Collect " + item) ;
            }

            Console.WriteLine("Should have work");

            var linkedlist = new actLinkedList<string>();
            for (int i = 0; i < 100; i++)
            {
                SendMessageTo(Tuple.Create(bhvLinkedListOperation.Add, i.ToString()),linkedlist);
            }

            new actEchoActor<Tuple<bhvLinkedListOperation, string>>(linkedlist, Tuple.Create(bhvLinkedListOperation.First, "5"));

            new actRing(1000,1000); // 30 sec

            // new actRing(100, 100);

            new actLinkedList<string>();

            IActor aServer = new actEchoServer();
            for (int i = 0; i < 100; i++)
            {
                actEchoClient aClient = new actEchoClient();// new actEchoClient(aServer);
                // DirectoryRequest.SendRegister("client + " + i.ToString(), aClient);
                aClient.Connect("EchoServer");
                aClient.SendMessage("client-" + i.ToString());
                // aClient.Disconnect();
            }
            var end = DateTime.UtcNow.Ticks;
            Console.WriteLine("All client allocated {0}", (double)(end - start) / 10000.0);

            // basic redirection
            IActor target = new actActor(new bhvBehavior<string>(t => { Console.WriteLine(t); }));
            IActor middle = new actActor(new bhvBehavior<string>(t => { t = t + " augmenté"; }));
            ((actActor)middle).RedirectTo(target);
            SendMessageTo("Bonjour", middle);


        }
   }

}
