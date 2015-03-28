﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{

    public class actDirectory : actActor
    {
        public enum DirectoryRequest { reqFind } ;
        private Dictionary<string, IActor> fDictionary = new Dictionary<string, IActor>();
        private static Lazy<actDirectory> fDirectory = new Lazy<actDirectory>(() => new actDirectory(), true);
        public actDirectory()
            : base()
        {
            Console.WriteLine("Dictionary Start and autoRegister");
            fDictionary.Add("Directory", this);

            Behaviors bhvs = new Behaviors();
            bhvs.AddBehavior(new bhvAction<IActor>()) ;
            bhvs.AddBehavior(new bhvAction<IActor,string>());
            BecomeMany(bhvs);
        }

        public static actDirectory GetDirectory()
        {
            return fDirectory.Value;
        }

        public string Stat()
        {
            return "Directory entries " + fDictionary.Count().ToString();
        }

        public void Disco(IActor anActor)
        {
            SendMessageTo(new Tuple<Action<IActor>,IActor>(DoDisco, anActor));
        }

        public void Register(IActor anActor, string aKey)
        {
            SendMessageTo(new Tuple<Action<IActor, string>, IActor, string>(DoRegister, anActor, aKey));
        }

        public void Find(IActor anActor, string aKey)
        {
            SendMessageTo(new Tuple<Action<IActor, string>, IActor, string>(DoFind, anActor, aKey));
        }

        private void DoDisco(IActor anActor)
        {
            List<string> directoryList = new List<string>();
            var localhost = Dns.GetHostName();
            var servername = ActorServer.GetInstance().Name;
            var prefix = "http://";
            var suffix = ":" + ActorServer.GetInstance().Port.ToString();
            var fullhost = prefix + localhost + suffix + "/" + servername + "/";
            foreach (string key in fDictionary.Keys)
            {
                var value = fDictionary[key];
                directoryList.Add(key + "=" + fullhost + value.Tag.Id);
            }
            SendMessageTo(directoryList, anActor);
        }

        private void DoRegister(IActor anActor,string msg)
        {
            if (fDictionary.Keys.Any(t => t == msg) == false )
                fDictionary.Add(msg,anActor);
        }

        private void DoFind(IActor anActor,string msg)
        {
            // Exists
            IActor Relative = null;
            if (fDictionary.TryGetValue(msg, out Relative))
            {
                SendMessageTo(Tuple.Create(DirectoryRequest.reqFind,Relative), anActor);
            }
            else
            {
                SendMessageTo(new Tuple<DirectoryRequest,IActor>(DirectoryRequest.reqFind, null), anActor);
            }
        }

    }

}


