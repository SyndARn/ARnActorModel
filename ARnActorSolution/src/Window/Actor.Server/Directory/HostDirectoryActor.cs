using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Actor.Base;
using System.Globalization;

namespace Actor.Server
{
    public class HostDirectoryActor : BaseActor
    {
        private readonly Dictionary<string, IActor> _uri2Actor = new Dictionary<string, IActor>(); // actor hosted
        private static readonly Lazy<HostDirectoryActor> _hostDirectory = new Lazy<HostDirectoryActor>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static HostDirectoryActor GetInstance() => _hostDirectory.Value;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetStat() => "Host entries " + _uri2Actor.Count.ToString(CultureInfo.InvariantCulture);

        internal void DoStat(IActor sender)
        {
            CheckArg.Actor(sender);
            sender.SendMessage("Host entries " + _uri2Actor.Count.ToString(CultureInfo.InvariantCulture));
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
            var entries = _uri2Actor.Keys.AsEnumerable<string>();
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
            if (!_uri2Actor.TryGetValue(lKey, out IActor lActor))
            {
                return;
            }

            lActor.SendMessage(aMsg.Data);
        }

        public static async Task<string> Stat(IActor sender)
        {
            CheckArg.Actor(sender);

            GetInstance().SendMessage((Action<IActor>) GetInstance().DoStat, sender);

            var task = await GetInstance()
                .ReceiveAsync(ans => ans is IActor actor && (sender.Equals(actor))).ConfigureAwait(false);
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
            _uri2Actor[anActor.Tag.Key()] = anActor ;
        }

        internal void DoUnregister(IActor anActor)
        {
            CheckArg.Actor(anActor);
            _uri2Actor.Remove(anActor.Tag.Key());
        }
    }
}
