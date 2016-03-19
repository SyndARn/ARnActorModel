using Actor.Base;
using Actor.Util;
using ARnAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentTestApplication
{

    /* 
     *  Initial
     *    requestprice
     *       PriceReceive
     *         ComputeOrder
     *             WaitConfirmation
     *             PriceReceive
     * 
    */

            public class WaitConfirmation : wfwStatus<MarketMessage>
    {
        public WaitConfirmation() : base()
        {
            Current = "WaitConfirmation" ;
        }
    }


            public class ComputeOrder : wfwStatus<MarketMessage>
    {
        public ComputeOrder() : base()
        {
            Current = "ComputeOrder" ;
        }
    }


        public class PriceRequested : wfwStatus<MarketMessage>
    {
        public PriceRequested() : base()
        {
            Current = "PriceRequested" ;
        }
    }

        public class PriceReceived : wfwStatus<MarketMessage>
    {
        public PriceReceived() : base()
        {
            Current = "PriceReceived" ;
        }
    }

    public class wfwPriceReceived : wfwStatus<MarketMessage>
    {
        public wfwPriceReceived() : base()
        {
            Current = "PriceReceived" ;
            var action = new bhvBehavior<IwfwStatus<MarketMessage>>
            (
                (t) =>
                {
                    return t.Current == "PriceReceived" &&
                        (t.Data.MarketOrder == MarketOrderType.Price);
                },
                (t) => 
                { 
                    // price receive
                } 
            ) ;                
            var tr = new wfwTransition<MarketMessage>()
            {
                Action = action,
                Destination = new wfwPriceReceived()
            } ;
        }
    }

    public class wfwStatusInitial : wfwStatus<MarketMessage>
    {
        public wfwStatusInitial() : base()
        {
            Current = "Initial" ;
            var action = new bhvBehavior<IwfwStatus<MarketMessage>>
            (
                (t) => { return t.Current == "Initial" ;},
                (t) => 
                { 
                    // request price 
                } 
            ) ;                
            var tr = new wfwTransition<MarketMessage>()
            {
                Action = action,
                Destination = new wfwPriceReceived()
            } ;
        }
    }


    public enum MarketOrderType {None,Buy,Sell,Price,Confirm}

    public class MarketMessage
    {
        public MarketOrderType MarketOrder {get ; set;}
        public IAgent Who;
        public Int64 Price;
        public Int64 Quantity;
        public Int64 Iter;
    }

    public class wfwTraderAgent : actAgent
    {
        private Int64 Share = 10;
        private Int64 Cash = 10000;
        private Int64 Price = 1000;
        private Random rnd = new Random(System.Environment.TickCount);
        IAgent ticker;
        IAgent market;
        actWorkflow<MarketMessage> Workflow ;

        public wfwTraderAgent(string agentName, IAgent aticker, IAgent aMarket)
            : base()
        {
            ticker = aticker;
            market = aMarket;
            Workflow = new actWorkflow<MarketMessage>(new wfwStatusInitial());
            Start(agentName);
        }

        public override void InitState(string anAgentName)
        {
            base.InitState(anAgentName);
        }

        private void ReceivePrice(Tuple<DeltaTime, Int64> msg)
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
                            market.SendMessage(BuySellOrder.CastBuy(this, Price));
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
            dtm.Sender.SendMessage(new DeltaTimeAckMessage(dtm, this));
        }

        public override string GetObserveMessage(long dt)
        {
            return string.Empty;
            // return string.Format("Cash {0} Share {1} Price {2}", Cash, Share, Price);
        }

    }


}
