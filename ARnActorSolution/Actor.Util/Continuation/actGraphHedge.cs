using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base; 

namespace Actor.Util
{

    public class actGraphHedge<T>
    {
        public actGraphNode<T> End { get; private set; }
        public string Data { get; private set;}
    }

    public enum GNRequest { None, Diffuse }

    public class actGraphNode<T> : actActor
    {
        public IEnumerable<actGraphHedge<T>> Hedges { get; private set; }
        private void DoDiffuse(Tuple<GNRequest,T> msg)
        {
            if (msg.Item1 == GNRequest.Diffuse)
            {
                foreach(var item in Hedges)
                {
                    item.End.SendMessage(msg);
                }
            }
        }
    }

    /*
     * send message to actGraphNode
     * process with the message
     * send along hedges, now hedge are behavior
     */

}
