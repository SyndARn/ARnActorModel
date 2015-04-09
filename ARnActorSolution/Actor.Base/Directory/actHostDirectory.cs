using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Xml;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Net;


namespace Actor.Base
{
    public class actHostDirectory : actActor 
    {
        private Dictionary<String, WeakReference<IActor>> fUri2Actor = new Dictionary<String, WeakReference<IActor>>(); // actor hosted
        private static Lazy<actHostDirectory> fHostDirectory = new Lazy<actHostDirectory>();

        public static actHostDirectory GetInstance()
        {
            return fHostDirectory.Value;
        }

        public string GetStat()
        {
            return "Host entries " + fUri2Actor.Count.ToString();
        }

        public void DoStat(IActor sender)
        {
            sender.SendMessage("Host entries " + fUri2Actor.Count.ToString());
        }

        public actHostDirectory()
            : base()
        {
            var behaviors = new Behaviors();
            behaviors.AddBehavior(new bhvAction<SerialObject>());
            behaviors.AddBehavior(new bhvAction<IActor>());
            behaviors.AddBehavior(new bhvAction());
            behaviors.AddBehavior(new bhvBehavior<SerialObject>(DoRouting));
            BecomeMany(behaviors);
        }

        private void DoRouting(SerialObject aMsg)
        {
            // find host in host directory
            WeakReference<IActor> lWeakActor = null ;
            // get id from uri
            var lKey = aMsg.Tag.Key();
            if (fUri2Actor.TryGetValue(lKey, out lWeakActor))
            {
                IActor lActor = null;
                if (lWeakActor.TryGetTarget(out lActor))
                  lActor.SendMessage(aMsg.Data);
            }
        }

        public static async Task<string> Stat(IActor sender)
        {
            actHostDirectory.GetInstance().SendMessage(new Tuple<Action<IActor>, IActor>(actHostDirectory.GetInstance().DoStat, sender));

            var task = await actHostDirectory.GetInstance()
                .Receive(ans => { return (ans is IActor) && (sender.Equals(((IActor)ans))); }) ;
            return task as string ;
        }

        public static void Register(IActor anActor)
        {
            actHostDirectory.GetInstance().SendMessage(new Tuple<Action<IActor>, IActor>(actHostDirectory.GetInstance().DoRegister, anActor)); 
        }

        public static void Unregister(IActor anActor)
        {
            actHostDirectory.GetInstance().SendMessage(new Tuple<Action<IActor>, IActor>(actHostDirectory.GetInstance().DoUnregister, anActor));
        }

        public void DoRegister(IActor anActor)
        {
            fUri2Actor[anActor.Tag.Key()] = new WeakReference<IActor>(anActor) ;
        }

        public void DoUnregister(IActor anActor)
        {
            fUri2Actor.Remove(anActor.Tag.Key());
        }


    }
}
