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

    public static class Centroid<K>
    {
        public static K Calc(IEnumerable<K> keys)
        {
            CheckArg.IEnumerable(keys);
            var dic = new Dictionary<HashKey, K>();
            foreach (var k in keys)
            {
                dic[HashKey.ComputeHash(k.ToString())] = k;
            }
            var elected = dic.Keys.OrderBy(h => h.ToString()).FirstOrDefault();
            return dic[elected];
        }
    }

    public enum AgentStatus { Start, PeekKey, NearestNode, AddKey }

    public class AgentActor<K, V> : BaseActor
    {
        public AgentActor()
        {
            Become(new Behavior<IPeerActor<K, V>>(
            a =>
            {
                var keys = a.AskKeys();
                var peers = a.AskPeers();
                // peek key out of centroid
                var key = Centroid<K>.Calc(keys.Result());
                // calc nearest peer
                var orderedPeers = peers.Result().OrderBy(n => n.Item1.ToString());
                var hashKey = HashKey.ComputeHash(key.ToString());

                foreach (var peer in orderedPeers)
                {
                    if (hashKey.CompareTo(peer.Item1) > 0)
                    {
                        // deposit
                        // get current K V
                        var future = new Future<V>();
                        a.GetNode(key, future);
                        var result = future.Result();
                        if (result != null)
                        {
                            // set current K V
                            (peer.Item2 as IPeerActor<K, V>).StoreNode(key, future.Result());
                        }
                        break;
                    }
                }
            }));
        }
    }

    public class AgentPeerActor<K, V> : BaseActor
    {
        // call host, update local centroid, update evap
    }

    public class AgentDiscoActor<K, V> : BaseActor
    {
        private int sygmergy = 10;

        // go to another host, ask prev and succ, update this
        public AgentDiscoActor() : base()
        {
            Become(new Behavior<IPeerActor<K, V>>(a =>
            {               
                // get current nodes
                var peers = a.AskPeers();
                // take one of them (lower than your reference node)
                // deposit sygmergy
                // go to this one
                // until sygmergy is down
            }));
        }
    }

    public class AgentCleanHost<K, V> : BaseActor
    {
        // look local host , remove evap < 0
    }
}
