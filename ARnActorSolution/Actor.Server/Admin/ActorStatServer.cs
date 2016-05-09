using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actor.Server
{
    public class ActorStatServer : BaseActor
    {
        public ActorStatServer()
        {
            Become(new Behavior<IActor>(
                msg => { return (msg is IActor) ;},
            Behavior)) ;
        }
        private void Behavior(IActor msg)
        {
            // get number of actor in directory
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(DirectoryActor.GetDirectory().Stat());
            // get number of actor in queue list
            sb.AppendLine(ActorTask.Stat()) ;
            // get number of actor in hostdirectory
            sb.AppendLine(HostDirectoryActor.GetInstance().GetStat());
            msg.SendMessage(sb.ToString()) ;
            Become(new NullBehaviors());
        }
    }
}

