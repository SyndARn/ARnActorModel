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
        private readonly Dictionary<String, IActor> fUri2Actor = new Dictionary<String, IActor>(); // actor hosted
        private static Lazy<HostDirectoryActor> fHostDirectory = new Lazy<HostDirectoryActor>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static HostDirectoryActor GetInstance()
        {
            return fHostDirectory.Value;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetStat()
        {
            return "Host entries " + fUri2Actor.Count.ToString(CultureInfo.InvariantCulture);
        }

        internal void DoStat(IActor sender)
        {
            CheckArg.Actor(sender);
            sender.SendMessage("Host entries " + fUri2Actor.Count.ToString(CultureInfo.InvariantCulture));
        }

        public IEnumerable<string> GetEntries()
        {
            var future = new Future<IEnumerable<string>>();
            GetEntries(future);
            return future.Result();
        }

        private void GetEntries(IActor actor)
        {
            CheckArg.Actor(actor);
            var entries = fUri2Actor.Keys.AsEnumerable<string>();
            actor.SendMessage(entries);
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
            // get id from uri
            var lKey = aMsg.Tag.Key();
            if (fUri2Actor.TryGetValue(lKey, out IActor lActor))
            {
                lActor.SendMessage(aMsg.Data);
            }
        }

        public static async Task<string> Stat(IActor sender)
        {
            CheckArg.Actor(sender);

            GetInstance().SendMessage((Action<IActor>) GetInstance().DoStat, sender);

            var task = await HostDirectoryActor.GetInstance()
                .ReceiveAsync(ans => { return (ans is IActor) && (sender.Equals(((IActor)ans))); }) ;
            return task as string ;
        }

        public static void Register(IActor anActor)
        {
            CheckArg.Actor(anActor);
            GetInstance().SendMessage((Action<IActor>)GetInstance().DoRegister, anActor); 
        }

        public static void Unregister(IActor anActor)
        {
            CheckArg.Actor(anActor);
            GetInstance().SendMessage((Action<IActor>)GetInstance().DoUnregister, anActor);
        }

        internal void DoRegister(IActor anActor)
        {
            CheckArg.Actor(anActor);
            fUri2Actor[anActor.Tag.Key()] = anActor ;
        }

        internal void DoUnregister(IActor anActor)
        {
            CheckArg.Actor(anActor);
            fUri2Actor.Remove(anActor.Tag.Key());
        }


    }
}
