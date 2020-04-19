using System;
using System.Collections.Generic;
using Actor.Base;
using Actor.Server;
using Actor.Util;
using Actor.Service;
using System.Globalization;

namespace Actor.TestApplication
{
    internal class ActorMain : BaseActor
    {
        private CollectionActor<string> _collect;

        public ActorMain() : base()
        {
            Become(new Behavior<string>(t => true, DoBehavior));
            new ActorService();
            SendMessage("Start");
        }

        private List<IActor> _clientList;

        private void DoBehavior(string msg)
        {
            Console.WriteLine("Serv Start");
            long start = DateTime.UtcNow.Ticks;
            _collect = new CollectionActor<string>();
            List<string> list = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                _collect.Add(i.ToString(CultureInfo.InvariantCulture));
                list.Add(i.ToString(CultureInfo.InvariantCulture));
            }

            foreach (string item in _collect)
            {
                Console.WriteLine("Collect " + item);
            }

            BaseActor actForeach = new BaseActor(new ForEachBehavior<string>());
            actForeach.SendMessage<IEnumerable<string>, Action<String>>(list,
                t => Console.WriteLine("list " + t));

            Console.WriteLine("Should have work");

            LinkedListActor<string> linkedlist = new LinkedListActor<string>();
            for (int i = 0; i < 100; i++)
            {
                linkedlist.SendMessage(LinkedListOperation.Add, i.ToString());
            }

            new EchoActor<Tuple<LinkedListOperation, string>>(linkedlist, Tuple.Create(LinkedListOperation.First, "5"));

            new RingActor(1000,1000); // 10 sec

            new LinkedListActor<string>();

            IActor aServer = new EchoServerActor();
            _clientList = new List<IActor>();
            for (int i = 0; i < 100; i++)
            {
                EchoClientActor aClient = new EchoClientActor();// new actEchoClient(aServer);
                // DirectoryRequest.SendRegister("client + " + i.ToString(), aClient);
                aClient.Connect("EchoServer");
                aClient.SendMessage("client-" + i.ToString(CultureInfo.InvariantCulture));
                _clientList.Add(aClient);
                // aClient.Disconnect();
            }
            long end = DateTime.UtcNow.Ticks;
            Console.WriteLine("All client allocated {0}", (end - start) / 10000.0);

            // basic redirection
            IActor target = new BaseActor(new Behavior<string>(t => Console.WriteLine(t)));
            IActor middle = new BaseActor(new Behavior<string>(t => t += " augmenté"));
            middle.SendMessage("Bonjour");
        }
   }
}
