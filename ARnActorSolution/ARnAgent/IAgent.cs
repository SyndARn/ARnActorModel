using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARnAgent
{
    public interface IAgent : IActor
    {
        void InitState(string anAgentName) ;
        void React(DeltaTimeMessage Dtm) ;
        // void Observe(Tuple<IActor, DeltaTime> dt);
        string GetObserveMessage(long dt);
        void Kill() ;
    }
}
