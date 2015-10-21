using Actor.Base;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{
    public class ActorStatServer : actActor
    {
        public ActorStatServer()
        {
            Become(new bhvBehavior<IActor>(
                msg => { return (msg is IActor) ;},
            Behavior)) ;
        }
        private void Behavior(IActor msg)
        {
            // get number of actor in directory
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(actDirectory.GetDirectory().Stat());
            // get number of actor in queue list
            sb.AppendLine(ActorTask.Stat()) ;
            // get number of actor in hostdirectory
            sb.AppendLine(actHostDirectory.GetInstance().GetStat());
            msg.SendMessage(sb.ToString()) ;
            Become(null);
        }
    }
}

