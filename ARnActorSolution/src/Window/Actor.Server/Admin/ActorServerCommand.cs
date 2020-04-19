using System;
using System.Collections.Generic;
using Actor.Base;
using System.Linq;

namespace Actor.Server
{
    public class StatServerCommand : Behavior<string, IActor>, IActorServerCommand
    {
        public string Key => Name;

        public static string Name => "Stat";

        private void DoRun(IActor actor)
        {
            ActorStatServer sa = new ActorStatServer();
            sa.SendMessage(actor);
        }

        public StatServerCommand() : base()
        {
            Pattern = (k,a) => k == Name;
            Apply = (k,a) => DoRun(a);
        }
    }
}