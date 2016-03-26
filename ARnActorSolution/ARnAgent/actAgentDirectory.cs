using Actor.Base;
using Actor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARnAgent
{
    public class actAgentDirectory : BaseActor
    {
        private List<IAgent> agentList = new List<IAgent>();

        public actAgentDirectory() : base()
        {
            var bhv1 = new bhvBehavior<IAgent>(DoRegister);
            var bhv2 = new bhvBehavior<DeltaTime>(DoSend);
            var bhv3 = new bhvBehavior<Tuple<IActor,DeltaTime>>(DoObserve);
            var bhv4 = new bhvBehavior<DeltaTimeAckMessage>(DoAck);
            var bhvall = new Behaviors();
            bhvall.AddBehavior(bhv1);
            bhvall.AddBehavior(bhv2);
            bhvall.AddBehavior(bhv3);
            bhvall.AddBehavior(bhv4);
            BecomeMany(bhvall);
            actDirectory.GetDirectory().Register(this, "AgentDirectory");
        }

        private void DoRegister(IAgent anAgent)
        {
            agentList.Add(anAgent);
        }

        private void DoObserve(Tuple<IActor,DeltaTime> msg)
        {
            new actBroadCast<Tuple<IActor, DeltaTime>>().BroadCast(msg, agentList);
        }

        private void DoAck(DeltaTimeAckMessage dtm)
        {
        }

        private void DoSend(DeltaTime dt)
        {
                DeltaTimeMessage dtm = DeltaTimeMessage.CastMessage(this, dt);
                new actBroadCast<DeltaTimeMessage>().BroadCast(dtm, agentList);
        }
    }
}
