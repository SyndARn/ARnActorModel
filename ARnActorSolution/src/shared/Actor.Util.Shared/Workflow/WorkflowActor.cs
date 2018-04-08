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
     *  find a suitable transition
     * apply the elected transition
     * 
     * from the calling actor pov 
     * 
     * behavior are Workflow and GetStatus
     * Workflow on a wfwMessage pattern election get a new status
     * GetStatus answer with the current status
    */
    public class WorkflowActor<T> : BaseActor
    {
        private IWfwStatus<T> fCurrent ;
        public WorkflowActor(IWfwStatus<T> startWith)
            : base()
        {
            fCurrent = startWith;
            Become(new Behavior<IWfwStatus<T>>(DoProcess));
        }
        private void DoProcess(IWfwStatus<T> aStatus)
        {
            // find transition
            foreach(var tr in aStatus.TransitionList)
            {
                if (tr.Action.Pattern(aStatus))
                {
                    tr.Action.Apply(aStatus);
                    // change status
                    fCurrent = tr.Destination;
                    break ;
                }
            }
        }
    }

    public class WfwTransition<T> : IWfwTransition<T>
    {

        public IWfwStatus<T> Destination {get; set;}

        public Behavior<IWfwStatus<T>> Action
        {
            get;
            set;
        }
    }

    public class WfwStatus<T> : IWfwStatus<T>
    {
        public List<IWfwTransition<T>> TransitionList { get; private set; }
        public string Current { get; protected set; }
        public WfwStatus()
        {
            TransitionList = new List<IWfwTransition<T>>() ;
            Current = string.Empty;
        }
        public T Data { get; protected set; }
    }

    public interface IWfwTransition<T>
    {
        IWfwStatus<T> Destination { get; }
        Behavior<IWfwStatus<T>> Action { get; }
    }

    public interface IWfwStatus<T>
    {
        string Current {get ;}
        List<IWfwTransition<T>> TransitionList { get; }
        T Data { get; }
    }

}
