using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using ARnAgent;
using Actor.Util;

namespace AgentTestApplication
{
    public class TickerAgent : actAgent 
    {

        private Int64 fLastPrice;
        private DeltaTime delta;

        public TickerAgent(): base()
        {
            Start("TickerAgent");
        }

        public override void InitState(string anAgentName)
        {
            base.InitState(anAgentName);
            DirectoryActor.GetDirectory().Register(this, "TickerAgent");
            AddBehavior(new Behavior<Int64>(DoTick));
            AddBehavior(new Behavior<IActor>(DoPriceRequest));
        }

        public override void React(DeltaTimeMessage dtm)
        {
            delta = dtm.Dt;
            dtm.Sender.SendMessage(new DeltaTimeAckMessage(dtm, this));
        }

        public override string GetObserveMessage(long dt)
        {
            return string.Format("Price {0} ", fLastPrice);
        }

        private void DoTick(Int64 aPrice)
        {
            fLastPrice = aPrice;
        }

        private void DoPriceRequest(IActor anActor)
        {
            anActor.SendMessage(Tuple.Create(delta, fLastPrice));
        }
    }
}
