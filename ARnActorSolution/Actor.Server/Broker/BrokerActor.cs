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
    public enum BrokerAction { RegisterWorker, UnRegisterWorker, Hearbeat, Start };
    public enum WorkerReadyState { Unknown, Idle, Busy, Transient };
    public enum RequestReadyState { Unknown, Running,Done,Todo };

    public struct WorkerStatus
    {
        public WorkerReadyState State;
        public int TTL;
    }

    public struct RequestStatus<T>
    {
        public T Data;
        public RequestReadyState State;
    }

    public class BrokerActor<T> : BaseActor
    {
        private Dictionary<IActor, WorkerStatus> fWorkers = new Dictionary<IActor, WorkerStatus>();
        private Dictionary<ActorTag, RequestStatus<T>> fRequests = new Dictionary<ActorTag, RequestStatus<T>>();
        private int fLastWorkerUsed = 0;
        private int fTTL = 0;
        private int fRequestProcessed = 0;

        public IActor fLogger { get; set; }

        public void RegisterWorker(WorkerActor<T> worker)
        {
            this.SendMessage(BrokerAction.RegisterWorker, (IActor)worker);
        }

        private void Log(string toLog)
        {
            if (fLogger != null)
                fLogger.SendMessage(toLog);
        }

        private void Log(string toLog, object[] args)
        {
            if (fLogger != null)
                fLogger.SendMessage(string.Format(toLog, args));
        }

        private void Log(string toLog, object arg0)
        {
            if (fLogger != null)
                fLogger.SendMessage(string.Format(toLog, arg0));
        }

        private void Log(string toLog, object arg0, object arg1)
        {
            if (fLogger != null)
                fLogger.SendMessage(string.Format(toLog, arg0, arg1));
        }

        public BrokerActor() : base()
        {
            // logger

            // heartbeat actor
            Become(new Behavior<BrokerAction>(
                s => s == BrokerAction.Start,
                s =>
            {
                var actor = new HeartBeatActor(30000);
                actor.SendMessage((IActor)this);
                Log("HeartBeat start");
            }));

            // register worker
            AddBehavior(new Behavior<BrokerAction, IActor>
                (
                (b, a) => b == BrokerAction.RegisterWorker,
                 (b, a) =>
                 {
                     WorkerStatus workerStatus;
                     workerStatus.TTL = 0;
                     workerStatus.State = WorkerReadyState.Idle;
                     fWorkers.Add(a, workerStatus);
                     Log("Worker Register", a.Tag.Id);
                 }
                ));
            // unregister worker
            AddBehavior(new Behavior<BrokerAction, IActor>
                (
                (b, a) => b == BrokerAction.UnRegisterWorker,
                 (b, a) =>
                 {
                     fWorkers.Remove(a);
                     Log("Worker UnRegister", a.Tag.Id);
                 }
                ));
            // process client request
            AddBehavior(new Behavior<T>(
                (t) => true,
                (t) =>
                {
                    var tag = new ActorTag();
                    var requestStatus = new RequestStatus<T>();
                    requestStatus.Data = t;
                    requestStatus.State = RequestReadyState.Todo;
                    fRequests[tag] = requestStatus;
                    fLastWorkerUsed++;
                    if (fLastWorkerUsed < 0)
                        fLastWorkerUsed = 0;
                    else
                        if (fLastWorkerUsed >= fWorkers.Count)
                        fLastWorkerUsed = 0;
                    var worker = fWorkers.Where(w => w.Value.State == WorkerReadyState.Idle).Skip(fLastWorkerUsed).FirstOrDefault();
                    if (worker.Key == null)
                    {
                        worker = fWorkers.FirstOrDefault(w => w.Value.State == WorkerReadyState.Idle);
                    }
                    if (worker.Key != null)
                    {
                        var workerState = fWorkers[worker.Key];
                        workerState.State = WorkerReadyState.Busy;
                        workerState.TTL = 0;
                        fWorkers[worker.Key] = workerState;
                        requestStatus.State = RequestReadyState.Running;
                        fRequests[tag] = requestStatus;
                        worker.Key.SendMessage((IActor)this, tag, t);
                        Log("Processing Request {0} on worker {1}", tag.Id, worker.Key.Tag.Id);
                    }
                    else
                    {
                        Log("No idle worker for Request {0}", tag.Id);
                    }
                }));
            // worker refuse job
            AddBehavior(new Behavior<IActor, WorkerReadyState, ActorTag>
                (
                (a, s, t) => s == WorkerReadyState.Busy,
                 (a, s, t) =>
                 {
                     var workerState = fWorkers[a];
                     workerState.State = WorkerReadyState.Busy;
                     workerState.TTL = 0;
                     fWorkers[a] = workerState;
                     var requestStatus = fRequests[t];
                     requestStatus.State = RequestReadyState.Todo;
                     fRequests[t] = requestStatus;
                     Log("Worker {0} can't process request {1}", a.Tag.Id, t.Id);
                 }
                ));
            // worker finished job
            AddBehavior(new Behavior<IActor, WorkerReadyState, ActorTag>
                (
                (a, s, t) => s == WorkerReadyState.Idle,
                 (a, s, t) =>
                 {
                     fRequestProcessed++;
                     fRequests.Remove(t);
                     var workerState = fWorkers[a];
                     workerState.State = WorkerReadyState.Idle;
                     workerState.TTL = fTTL;
                     fWorkers[a] = workerState;
                     Log("Request {0} End on worker {1}", t.Id, a.Tag.Id);
                     // find another request
                     if (fRequests.Count > 0)
                     {
                         var newRequest = fRequests.FirstOrDefault(r => r.Value.State == RequestReadyState.Todo);
                         if (newRequest.Key != null)
                         {
                             workerState.State = WorkerReadyState.Busy;
                             workerState.TTL = 0;
                             fWorkers[a] = workerState;
                             var requestStatus = fRequests[newRequest.Key];
                             requestStatus.State = RequestReadyState.Running;
                             fRequests[newRequest.Key] = requestStatus;
                             a.SendMessage((IActor)this, newRequest.Key, newRequest.Value.Data);
                             Log("Processing Request {0} on worker {1}", newRequest.Key.Id, a.Tag.Id);
                         }
                     }
                 }
                ));
            // heartbeatactor
            AddBehavior(new Behavior<HeartBeatActor>
                (
                h =>
                {
                    foreach (var worker in fWorkers.Where(w => w.Value.TTL < fTTL))
                    {
                        worker.Key.SendMessage((IActor)this, BrokerAction.Hearbeat);
                    }
                    fTTL++;
                    Log(String.Format("Heart Beat Signal, Request Processed {0}", fRequestProcessed));
                }
                ));
            // heartbeat answer
            AddBehavior(new Behavior<IActor, WorkerReadyState>(
                (a, w) =>
                {
                    var workerState = fWorkers[a];
                    workerState.TTL = fTTL;
                    fWorkers[a] = workerState;
                    Log("Answer To HeartBeat from Worker {0}", a.Tag.Id);
                }));
            // start heart beat
            SendMessage(BrokerAction.Start);
        }
    }

    public class HeartBeatActor : BaseActor
    {
        private int fTimeOutMs;
        public HeartBeatActor(int timeOutMs)
        {
            fTimeOutMs = timeOutMs;
            Become(new Behavior<IActor>(a =>
                {
                    a.SendMessage(this);
                    Task.Delay(fTimeOutMs).Wait();
                    SendMessage(a);
                }));
        }
    }

    public abstract class WorkerActor<T> : BaseActor
    {
        private WorkerReadyState fState = WorkerReadyState.Idle;

        public WorkerActor() : base()
        {
            Become(new Behavior<IActor, ActorTag, T>
                (
                    (a, t, o) =>
                    {
                        switch (fState)
                        {
                            case WorkerReadyState.Busy:
                                {
                                    // send busy
                                    a.SendMessage((IActor)this, WorkerReadyState.Busy, t);
                                    break;
                                }
                            case WorkerReadyState.Idle:
                                {
                                    fState = WorkerReadyState.Busy;
                                    Task.Run(() =>
                                    {
                                        Process(o);
                                        fState = WorkerReadyState.Idle;
                                        a.SendMessage((IActor)this, WorkerReadyState.Idle, t);
                                    });
                                    break;
                                }
                        }
                    }
                ));

            AddBehavior(new Behavior<IActor, BrokerAction>(
                (a, b) => b == BrokerAction.Hearbeat,
                (a, b) =>
                {
                    a.SendMessage((IActor)this, fState);
                }));
        }

        protected abstract void Process(T aT);

    }

}
