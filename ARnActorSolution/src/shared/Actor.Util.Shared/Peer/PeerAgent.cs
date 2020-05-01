using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;
using System.Linq;

namespace Actor.Util
{
    // set a key,value hash the key with xor salt of well known things
    // find peer closest key ( that is me or next )
    // store key, value

    // find a key, hash the key
    // find peer closest key (that is me or next )
    // get key in this peer

    // find key value same but return key,value

    // start with one peer
    // add a peer either left or right
    // distribute key with an agent

    // calc at least 2 key value, peer / 4 for redundancy with different hash seeding

    public static class CenterKey
    {
        public static TKey Calc<TKey>(IEnumerable<TKey> keys)
        {
            CheckArg.IEnumerable(keys);
            Dictionary<HashKey, TKey> dic = new Dictionary<HashKey, TKey>();
            foreach (TKey k in keys)
            {
                dic[HashKey.ComputeHash(k.ToString())] = k;
            }
            HashKey elected = dic.Keys.OrderBy(h => h.ToString()).FirstOrDefault();
            return dic[elected];
        }
    }

    public enum AgentStatus { Start, PeekKey, NearestNode, AddKey }

    public class AgentActor<TKey, TValue> : BaseActor
    {
        public AgentActor()
        {
            Become(new Behavior<IPeerActor<TKey, TValue>>(
            a =>
            {
                IFuture<IEnumerable<TKey>> keys = a.AskKeys();
                IFuture<IEnumerable<IPeerActor<TKey>>> peers = a.AskPeers();
                // peek key out of centroid
                TKey key = CenterKey.Calc(keys.Result());
                // calc nearest peer
                IOrderedEnumerable<IPeerActor<TKey>> orderedPeers = peers.Result().OrderBy(n => n.GetPeerHashKey().ToString());
                HashKey hashKey = HashKey.ComputeHash(key.ToString());

                foreach (IPeerActor<TKey> peer in orderedPeers)
                {
                    if (hashKey.CompareTo(peer.GetPeerHashKey()) > 0)
                    {
                        // deposit
                        // get current K V
                        TValue result = a.GetNode(key).Result();
                        if (result != null)
                        {
                            // set current K V
                            (peer as IPeerActor<TKey, TValue>).StoreNode(key, result);
                        }
                        break;
                    }
                }
            }));
        }
    }

    public class AgentPeerActor<TKey, TValue> : BaseActor
    {
        // call host, update local centroid, update evap
    }

    public class AgentDiscoActor<TKey, TValue> : BaseActor
    {
        // private int sygmergy = 10;

        // go to another host, ask prev and succ, update this
        public AgentDiscoActor() : base()
        {
            Become(new Behavior<IPeerActor<TKey, TValue>>(a =>
            {
                // get current nodes
                IFuture<IEnumerable<IPeerActor<TKey>>> peers = a.AskPeers();
                // take one of them (lower than your reference node)
                // deposit sygmergy
                // go to this one
                // until sygmergy is down
            }));
        }
    }

    public class AgentCleanHost<TKey, TValue> : BaseActor
    {
        // look local host , remove evap < 0
    }
}
