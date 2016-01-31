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

    public enum BuySell { None, Buy, Sell } ;
    public class BuySellOrder
    {
        public BuySellOrder() { }
        public static BuySellOrder CastSell(IAgent aWho, Int64 aPrice)
        {
            var bs = new BuySellOrder()
            {
                BuyOrSell = BuySell.Sell,
                Who = aWho,
                Price = aPrice,
                Quantity = 1
            };
            return bs;
        }
        public static BuySellOrder CastBuy(IAgent aWho, Int64 aPrice)
        {
            var bs = new BuySellOrder()
            {
                BuyOrSell = BuySell.Buy,
                Who = aWho,
                Price = aPrice,
                Quantity = 1
            };
            return bs;
        }
        public BuySell BuyOrSell;
        public IAgent Who;
        public Int64 Price;
        public Int64 Quantity;
        public Int64 Iter;
    }

    /*
     *         void InitState(string anAgentName) ;
        void React(DeltaTime dt) ;
        // void Observe(Tuple<IActor, DeltaTime> dt);
        string GetObserveMessage(long dt);
        void Kill() ;
     * */


    public class BuyOrders
    {
        private SortedDictionary<Int64, List<BuySellOrder>> fOrders = new SortedDictionary<long, List<BuySellOrder>>();
        public BuyOrders() { }

        public void AddOrder(BuySellOrder anOrder)
        {
            List<BuySellOrder> aList;
            if (!fOrders.TryGetValue(anOrder.Price, out aList))
            {
                aList = new List<BuySellOrder>();
                fOrders[anOrder.Price] = aList;
            }
            fOrders[anOrder.Price].Add(anOrder);
        }

        public void RemoveOrder(BuySellOrder anOrder)
        {
            fOrders[anOrder.Price].Remove(anOrder);
            if (fOrders[anOrder.Price].Count == 0)
                fOrders.Remove(anOrder.Price);
        }

        public BuySellOrder GetOrder(Int64 aPrice)
        {
            if (fOrders.ContainsKey(aPrice))
                return fOrders[aPrice].FirstOrDefault();
            else
                return null;
        }

        public Int64 FirstPrice
        {
            get
            {
                return fOrders.Keys.FirstOrDefault();
            }
        }

        public Int64 LastPrice
        {
            get
            {
                return fOrders.Keys.LastOrDefault();
            }
        }
    }

    public class MarketAgent : actAgent
    {
        private Int64 fPortFolio;
        private Int64 fCash;
        private Int64 fDt;
        private Int64 fPrice;
        private IAgent fTickerAgent;

        private BuyOrders fBuyDic = new BuyOrders();
        private BuyOrders fSellDic = new BuyOrders();

        public override void InitState(string anAgentName)
        {
            base.InitState(anAgentName);
            actDirectory.GetDirectory().Register(this, "MarketAgent");
            AddBehavior(new bhvBehavior<BuySellOrder>(DoOrder));
        }

        public override string GetObserveMessage(long dt)
        {
            if (fBuyDic.LastPrice == null) return string.Empty;
            if (fSellDic.LastPrice == null) return string.Empty;
            return string.Format("Price {0} Buy {1} Sell {2}", fPrice, fBuyDic.LastPrice, fSellDic.FirstPrice);
        }

        public MarketAgent(IAgent ticker)
            : base()
        {
            fTickerAgent = ticker;
            Start("MarketAgent");
        }

        public override void React(DeltaTimeMessage dtm)
        {
            Resolve();
            dtm.Sender.SendMessage(new DeltaTimeAckMessage(dtm, this));
        }

        private void DoOrder(BuySellOrder anOrder)
        {
            switch (anOrder.BuyOrSell)
            {
                case BuySell.Buy:
                    fDt++;
                    // try to add

                    fBuyDic.AddOrder(anOrder);
                    break;
                case BuySell.Sell:
                    fDt++;

                    // try to add
                    fSellDic.AddOrder(anOrder);
                    break;
            }
        }

        private void Resolve()
        {
            while (true)
            {
                var priceSell = fSellDic.FirstPrice;
                if (priceSell == 0) break ;
                var priceBuy = fBuyDic.LastPrice;
                if (priceBuy == 0) break;
                if (priceBuy >= priceSell)
                {
                    var orderBuy = fBuyDic.GetOrder(priceBuy);
                    var orderSell = fSellDic.GetOrder(priceSell);
                    fPrice = priceSell;
                    fBuyDic.RemoveOrder(orderBuy);
                    fSellDic.RemoveOrder(orderSell);
                    fTickerAgent.SendMessage(fPrice);
                    orderBuy.Who.SendMessage(new Tuple<BuySell, Int64, Int64>(BuySell.Buy, fPrice, 1));
                    orderSell.Who.SendMessage(new Tuple<BuySell, Int64, Int64>(BuySell.Sell, fPrice, 1));
                } else
                {
                    break;
                }
            }
        }
    }

}
