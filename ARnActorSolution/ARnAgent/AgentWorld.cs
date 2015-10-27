using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARnAgent
{
    public class AgentWorld
    {
        private actAgentDirectory fAgentDirectory;
        DeltaTime dt;

        public AgentWorld()
        {
            fAgentDirectory = new actAgentDirectory();
            dt = new DeltaTime(0) ;
        }

        public void Step()
        {
            fAgentDirectory.SendMessage(dt);
            dt.asDt++;
            dt.asDateTime = DateTime.Now;
        }

        public void Observe()
        {
            var obs = new actAgentObserver();
            fAgentDirectory.SendMessage(new Tuple<IActor, DeltaTime>(obs, dt));
        }
    }
}
