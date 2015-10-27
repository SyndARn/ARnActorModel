using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace ARnAgent
{

    public class DeltaTime
    {
        public Int64 asDt;
        public DateTime asDateTime;
        public DeltaTime(Int64 aDt)
        {
            asDt = aDt;
            asDateTime = DateTime.Now;
        }
    }

    public class DeltaTimeMessage
    {
        public DeltaTime Dt;
        public IActor Sender;
        public static DeltaTimeMessage CastMessage(IActor aSender, DeltaTime aDt)
        {
            return new DeltaTimeMessage() { Sender = aSender, Dt = aDt};
        }
    }

    public class DeltaTimeAckMessage
    {
        public DeltaTimeMessage dtm;
        public IAgent Responder;
        public DeltaTimeAckMessage()
        {
        }
        public DeltaTimeAckMessage(DeltaTimeMessage aDtm, IAgent aResponder)
        {
            dtm = aDtm;
            Responder = aResponder;
        }
    }


}
