using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARnAgent;
using Actor.Base;
using Actor.Util;

namespace AgentTestApplication
{
    public class TestAgent : actAgent
    {
        private Int64 Share = 10;
        private Int64 Cash = 10000;
        private Int64 Price = 1000;
        private Random rnd = new Random(System.Environment.TickCount);
        IAgent ticker;
        IAgent market;

        public TestAgent(string agentName, IAgent aticker, IAgent aMarket) : base()
        {
            ticker = aticker;
            market = aMarket;
            Start(agentName);

        }

        public override void InitState(string anAgentName)
        {
            base.InitState(anAgentName);
        }

        private void ReceivePrice(Tuple<DeltaTime,Int64> msg)
        {
            if (msg.Item2 > 0)
            {
                Price = msg.Item2;
            }
        }

        private void CloseReact()
        {

        }

        public override void React(DeltaTimeMessage dtm)
        {
            switch (rnd.Next(3))
            // buy
            {
                case 0:
                    {
                        if (Cash >= (Price - 1))
                        {
                            Price--;
                            Cash = Cash - Price;
                            Share = Share + 1;
                            market.SendMessage(BuySellOrder.CastBuy(this, Price)) ;
                        }
                    } break;
                // sell
                case 1:
                    {
                        if (Share > 0)
                        {
                            Price++;
                            Cash = Cash + Price;
                            Share = Share - 1;
                            market.SendMessage(BuySellOrder.CastSell(this, Price));
                        }
                    } break;
                // find price
                case 2:
                    {
                        ticker.SendMessage((IActor)this);
                    } break;
            }
            dtm.Sender.SendMessage(new DeltaTimeAckMessage(dtm,this)) ;
        }

        public override string GetObserveMessage(long dt)
        {
            return string.Empty;
            // return string.Format("Cash {0} Share {1} Price {2}", Cash, Share, Price);
        }

    }
}
