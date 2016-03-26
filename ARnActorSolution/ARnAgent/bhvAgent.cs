using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARnAgent
{
    public class bhvAgent : Behavior<string>
    {
         public bhvAgent(string anAgentName)
            : base()
        {
            Pattern = t => { return true ; };
            Apply = DoInitState ;
        }

        private void DoInitState(string msg)
        {
            InitState(msg);
        }

        protected virtual void InitState(string msg)
        {
        }
    }
}
