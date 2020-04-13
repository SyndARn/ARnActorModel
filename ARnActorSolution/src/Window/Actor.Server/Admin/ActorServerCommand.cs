using Actor.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Actor.Server
{
    public interface IActorServerCommand
    {
        string Name { get; }
        void Run(params object[] data);
    }

    // Shard Stat Disco Other

    public class DiscoServerCommand : IActorServerCommand
    {
        public string Name => "Disco";

        public void Run(params object[] data)
        {
            if (data.Length <2)
            {
                throw new ActorException("2 params expected");
            }
            if (String.IsNullOrEmpty(data[1].ToString()))
            {
                DirectoryActor.GetDirectory().Disco((IActor)data[0]);
            }
            else
            {
                new DiscoveryActor(data[0].ToString());
            }
        }
    }

    public class StatServerCommand : IActorServerCommand
    {
        public string Name => "Stat";

        public void Run(params object[] data)
        {
            if (data.Length != 1)
            {
                throw new ActorException("expecting at least a param");
            }
            ActorStatServer sa = new ActorStatServer();
            sa.SendMessage(data[0]);
        }
    }
}