using System;
using System.Collections.Generic;
using System.Linq;
using Actor.Base;

namespace Actor.Server
{
    public class DiscoServerCommand : Behavior<string, IActor, string>, IActorServerCommand
    {
        public  string Key =>  Name ;

        public const string Name = "Disco";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1806:Ne pas ignorer les résultats des méthodes", Justification = "<En attente>")]
        private void DoRun(IActor actor, string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                DirectoryActor.GetDirectory().Disco(actor);
            }
            else
            {
                new DiscoveryActor(data);
            }
        }

        public DiscoServerCommand() : base()
        {
            Pattern = (k, a, d) => k == Name;
            Apply = (k,a, d) => DoRun(a,d);
        }
    }
}