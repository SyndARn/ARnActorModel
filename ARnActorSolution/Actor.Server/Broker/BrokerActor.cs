using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Server
{

    /// <summary>
    /// BrokerActor
    ///     Have an actor that upon receiving message will fan out this message to one or more of pre-registrered actor
    ///     BrokerActor are very usefull if they are registered, sending a message to the broker will forward it across the
    ///     shards.
    ///     
    ///     a simple broker send a message to one of it workers, 
    ///     a more complex broker will take a different rout : one of free worker, a new spawned worker, etc ...
    /// 
    ///     worker can be made simple, they just react to message
    ///     or complex : they become statefull and can alert the broker of their current status (alive, ready, busy ...)
    /// 
    /// </summary>
    /// 
    public enum BrokerAction { RegisterWorker, UnRegisterWorker };
    public enum WorkerState { Ready, Busy };

    public class BrokerActor : BaseActor
    {
        private List<IActor> fWorkers = new List<IActor>();
        private int fLastWorkerUsed = -1;

        public BrokerActor() : base()
        {
            Become(new Behavior<BrokerAction, IActor>
                (
                 (b,a) =>
                 {
                     fWorkers.Add(a);
                 }  
                )) ;

            AddBehavior(new Behavior<WorkerState, IActor>
                (
                (s,a) =>
                {
                    // if busy, try another worker
                }
                ));
            AddBehavior(new Behavior<Object>(
                (t) => true,
                (t) =>
                {
                    if (fLastWorkerUsed < 0)
                        fLastWorkerUsed = 0;
                    else
                        if (fLastWorkerUsed >= fWorkers.Count)
                        fLastWorkerUsed = 0;
                    fWorkers[fLastWorkerUsed].SendMessage(t);
                }));
        }
    }

    public class WorkerActor : BaseActor
    {
        private WorkerState fState = WorkerState.Ready;

        public WorkerActor() : base()
        {
            Become(new Behavior<IActor, Object>
                (
                    (a,o) =>
                    {
                        switch(fState)
                        {
                            case WorkerState.Busy: a.SendMessage((IActor)this,WorkerState.Busy); break;
                            case WorkerState.Ready:
                                {
                                    a.SendMessage((IActor)this, WorkerState.Busy);
                                    // do something with o
                                    break;
                                }
                        }
                    }
                ));
        }
    }

}
