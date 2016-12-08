using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;

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

    public class AgentActor<K, V> : BaseActor
    {
        // peek a key
        // go to nearest node
        // add key with prob ? // deposit evap 
    }

    public class AgentPeerActor<K, V> : BaseActor
    {
        // call host, update local centroid, update evap
    }

    public class AgentDiscoActor<K, V> : BaseActor
    {
        // go to another host, ask prev and succ, update this
    }

    public class AgentCleanHost<K, V> : BaseActor
    {
        // look local host , remove evap < 0
    }
}
