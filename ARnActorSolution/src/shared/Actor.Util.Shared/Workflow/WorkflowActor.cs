using System.Collections.Generic;
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
        private IWfwStatus<T> _current;
        public WorkflowActor(IWfwStatus<T> startWith)
            : base()
        {
            _current = startWith;
            Become(new Behavior<IWfwStatus<T>>(DoProcess));
        }

        private void DoProcess(IWfwStatus<T> aStatus)
        {
            // find transition
            foreach (var tr in aStatus.TransitionList)
            {
                if (tr.Action.Pattern(aStatus))
                {
                    tr.Action.Apply(aStatus);
                    // change status
                    _current = tr.Destination;
                    break;
                }
            }
        }
    }

    public interface IWfwTransition<T>
    {
        IWfwStatus<T> Destination { get; }
        Behavior<IWfwStatus<T>> Action { get; }
    }

    public interface IWfwStatus<T>
    {
        string Current { get; }
        List<IWfwTransition<T>> TransitionList { get; }
        T Data { get; }
    }
}
