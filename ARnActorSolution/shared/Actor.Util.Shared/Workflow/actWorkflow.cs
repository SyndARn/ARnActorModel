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
    public class actWorkflow<T> : BaseActor
    {
        private IwfwStatus<T> fCurrent ;
        public actWorkflow(IwfwStatus<T> startWith)
            : base()
        {
            fCurrent = startWith;
            Become(new Behavior<IwfwStatus<T>>(DoProcess));
        }
        private void DoProcess(IwfwStatus<T> aStatus)
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

    public class wfwTransition<T> : IwfwTransition<T>
    {

        public IwfwStatus<T> Destination {get; set;}

        public Behavior<IwfwStatus<T>> Action
        {
            get;
            set;
        }
    }

    public class wfwStatus<T> : IwfwStatus<T>
    {
        public List<IwfwTransition<T>> TransitionList { get; private set; }
        public string Current { get; protected set; }
        public wfwStatus()
        {
            TransitionList = new List<IwfwTransition<T>>() ;
            Current = string.Empty;
        }
        public T Data { get; protected set; }
    }

    public interface IwfwTransition<T>
    {
        IwfwStatus<T> Destination { get; }
        Behavior<IwfwStatus<T>> Action { get; }
    }

    public interface IwfwStatus<T>
    {
        string Current {get ;}
        List<IwfwTransition<T>> TransitionList { get; }
        T Data { get; }
    }

}
