using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base; 

namespace Actor.Util
{
    /*
    // workflow 
     * status and transition
     * status is ... a string
     * transition are current behaviors
     * on reception of a message
     * one of the behaviors is elected (by pattern matching ...)
     * the apply change the status and anything else needed
     * pattern : accept workflow message
     * 
     *  find a sutable transition
     * apply the elected transition
     * 
     * from the calling actor pov 
     * 
     * behavior are Workflow and GetStatus
     * Workflow on a wfwMessage pattern election get a new status
     * GetStatus answer with the current status
    */
    public class actWorkflow : actActor
    {
        private wfwStatus fStart;
        public actWorkflow() : base()
        {
            BecomeMany(new wfwStatus());
        }
    }

    public class wfwTransition : bhvBehavior<wfwMessage>
    {
    }

    public class wfwStatusStart : wfwTransition
    {
        public wfwStatusStart() : base()
        {
            Pattern = t => false ;
            Apply = null ;
        }
    }

    public class wfwStatus : Behaviors
    {
        public wfwStatus()
        {

        }
    }

    public interface wfwMessage
    {
        string FromStatus {get ;} 
        string ToStatus {get ;}
    }

}
