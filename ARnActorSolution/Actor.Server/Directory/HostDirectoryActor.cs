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
using Actor.Base;
using System.Globalization;

namespace Actor.Server
{
    public class HostDirectoryActor : BaseActor 
    {
        private Dictionary<String, WeakReference<IActor>> fUri2Actor = new Dictionary<String, WeakReference<IActor>>(); // actor hosted
        private static Lazy<HostDirectoryActor> fHostDirectory = new Lazy<HostDirectoryActor>();

        public static HostDirectoryActor GetInstance()
        {
            return fHostDirectory.Value;
        }

        public string GetStat()
        {
            return "Host entries " + fUri2Actor.Count.ToString(CultureInfo.InvariantCulture);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Valider les arguments de méthodes publiques", MessageId = "0")]
        public void DoStat(IActor sender)
        {
            CheckArg.Actor(sender);
            sender.SendMessage("Host entries " + fUri2Actor.Count.ToString());
        }

        public HostDirectoryActor()
            : base()
        {
            var behaviors = new Behaviors();
            behaviors.AddBehavior(new ActionBehavior<SerialObject>());
            behaviors.AddBehavior(new ActionBehavior<IActor>());
            behaviors.AddBehavior(new ActionBehavior());
            behaviors.AddBehavior(new Behavior<SerialObject>(DoRouting));
            Become(behaviors);
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
            HostDirectoryActor.GetInstance().SendMessage(new Tuple<Action<IActor>, IActor>(HostDirectoryActor.GetInstance().DoStat, sender));

            var task = await HostDirectoryActor.GetInstance()
                .Receive(ans => { return (ans is IActor) && (sender.Equals(((IActor)ans))); }) ;
            return task as string ;
        }

        public static void Register(IActor anActor)
        {
            HostDirectoryActor.GetInstance().SendMessage(new Tuple<Action<IActor>, IActor>(HostDirectoryActor.GetInstance().DoRegister, anActor)); 
        }

        public static void Unregister(IActor anActor)
        {
            HostDirectoryActor.GetInstance().SendMessage(new Tuple<Action<IActor>, IActor>(HostDirectoryActor.GetInstance().DoUnregister, anActor));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Valider les arguments de méthodes publiques", MessageId = "0")]
        public void DoRegister(IActor anActor)
        {
            CheckArg.Actor(anActor);
            fUri2Actor[anActor.Tag.Key()] = new WeakReference<IActor>(anActor) ;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Valider les arguments de méthodes publiques", MessageId = "0")]
        public void DoUnregister(IActor anActor)
        {
            CheckArg.Actor(anActor, "anActor must exist");
            fUri2Actor.Remove(anActor.Tag.Key());
        }


    }
}
