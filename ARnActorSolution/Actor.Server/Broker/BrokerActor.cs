using System.Collections.Generic;
using System.Linq;
using Actor.Base;
using System.Globalization;

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
    public enum BrokerAction { RegisterWorker, UnregisterWorker, Hearbeat, Start };
    public enum WorkerReadyState { Unknown, Idle, Busy, Transient };
    public enum RequestState { Unprocessed, Processed, Running };

    public class RequestStatus<T>
    {
        public ActorTag Tag { get; set; }
        public RequestState State { get; set; }
        public T Data { get; set; }
    }

    public class WorkerStatus
    {
        public WorkerReadyState State { get; set; }
        public int TTL { get; set; }
    }

    public class BrokerActor<T> : BaseActor
    {
        private Dictionary<IActor, WorkerStatus> fWorkers = new Dictionary<IActor, WorkerStatus>();
        private Dictionary<ActorTag, RequestStatus<T>> fRequests = new Dictionary<ActorTag, RequestStatus<T>>();
        private int fLastWorkerUsed = 0;
        private int fTTL = 0;
        private int fRequestProcessed = 0;

        private IActor fLogger = new NullActor();
        public IActor Logger { get { return fLogger; } set { fLogger = value; } }

        public void RegisterWorker(IActor worker)
        {
            this.SendMessage(BrokerAction.RegisterWorker, worker);
        }

        private IActor FindWorker()
        {
            if (fLastWorkerUsed < 0)
            {
                fLastWorkerUsed = 0;
            }
            else
            {
                if (fLastWorkerUsed >= fWorkers.Count)
                { fLastWorkerUsed = 0; }
            }
            var worker = fWorkers.Where(w => w.Value.State == WorkerReadyState.Idle).Skip(fLastWorkerUsed).FirstOrDefault();
            fLastWorkerUsed++;
            if (worker.Key == null)
            {
                worker = fWorkers.FirstOrDefault(w => w.Value.State == WorkerReadyState.Idle);
            }
            return worker.Key;
        }

        private Behavior<BrokerAction> BehaviorBrokerStart()
        {
            return new Behavior<BrokerAction>(
                s => s == BrokerAction.Start,
                s =>
                {
                    var actor = new HeartBeatActor(30000);
                    actor.SendMessage((IActor)this);
                    Logger.SendMessage("HeartBeat start");
                });
        }

        private Behavior<BrokerAction, IActor> BehaviorRegisterWorker()
        {
            return new Behavior<BrokerAction, IActor>
                (
                (b, a) => b == BrokerAction.RegisterWorker,
                 (b, a) =>
                 {
                     WorkerStatus workerStatus = new WorkerStatus()
                     {
                         TTL = 0,
                         State = WorkerReadyState.Idle
                     };
                     fWorkers.Add(a, workerStatus);
                     LogString("Worker Register", a.Tag.Key());
                 }
                );
        }

        private Behavior<BrokerAction, IActor> BehaviorUnregisterWorker()
        {
            return new Behavior<BrokerAction, IActor>
                (
                (b, a) => b == BrokerAction.UnregisterWorker,
                 (b, a) =>
                 {
                     fWorkers.Remove(a);
                     LogString("Worker UnRegister", a.Tag.Key());
                 }
                );
        }

        public void LogString(string message)
        {
            Logger.SendMessage(message);
        }
        public void LogString(string message, object arg0)
        {
            Logger.SendMessage(string.Format(CultureInfo.InvariantCulture, message, arg0));
        }
        public void LogString(string message, object arg0, object arg1)
        {
            Logger.SendMessage(string.Format(CultureInfo.InvariantCulture, message, arg0, arg1));
        }
        public void LogString(string message, object[] args)
        {
            Logger.SendMessage(string.Format(CultureInfo.InvariantCulture, message, args));
        }

        private Behavior<T> BehaviorProcessClientRequest()
        {
            return new Behavior<T>(
                (t) =>
                {
                    var requestStatus = new RequestStatus<T>()
                    {
                        Data = t,
                        State = RequestState.Unprocessed,
                        Tag = new ActorTag()
                    };
                    fRequests[requestStatus.Tag] = requestStatus;

                    var worker = FindWorker();
                    if (worker != null)
                    {
                        fWorkers[worker].State = WorkerReadyState.Busy;
                        fWorkers[worker].TTL = 0;
                        worker.SendMessage((IActor)this, requestStatus.Tag, t);
                        requestStatus.State = RequestState.Running;
                        LogString("Processing Request {0} on worker {1}", requestStatus.Tag.Key(), worker.Tag.Key());
                    }
                    else
                    {
                        LogString("No available worker for Request {0}", requestStatus.Tag.Key());
                    }
                });
        }

        public BrokerActor() : base()
        {

            // heartbeat actor
            Become(BehaviorBrokerStart());

            // register worker
            AddBehavior(BehaviorRegisterWorker());

            // unregister worker
            AddBehavior(BehaviorUnregisterWorker());

            // process client request
            AddBehavior(BehaviorProcessClientRequest());

            // worker refuse job
            AddBehavior(new Behavior<IActor, WorkerReadyState, ActorTag>
                (
                (a, s, t) => s == WorkerReadyState.Busy,
                 (a, s, t) =>
                 {
                     fWorkers[a].State = WorkerReadyState.Busy;
                     LogString("Worker {0} can't process request {1}", a.Tag.Key(), t.Key());
                     var worker = FindWorker();
                     if (worker != null)
                     {
                         fWorkers[worker].State = WorkerReadyState.Busy;
                         fWorkers[worker].TTL = 0;
                         worker.SendMessage((IActor)this, t, fRequests[t]);
                         LogString("ReProcessing Request {0} on worker {1}", a.Tag.Key(), t.Key());
                     }
                     else
                     {
                         LogString("Wait for a worker for Request {0}", t.Key());
                     }
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
                     fWorkers[a].State = WorkerReadyState.Idle;
                     fWorkers[a].TTL = fTTL;
                     LogString("Request {0} End on worker {1}", t.Key(), a.Tag.Key());
                     // find a request
                     var tagRequest = fRequests.Values.FirstOrDefault(v => v.State == RequestState.Unprocessed);
                     if (tagRequest != null)
                     {
                         fWorkers[a].State = WorkerReadyState.Busy;
                         fWorkers[a].TTL = 0;
                         tagRequest.State = RequestState.Running;
                         a.SendMessage((IActor)this, tagRequest.Tag, tagRequest.Data);
                         LogString("Processing Request {0} reusing worker {1}", tagRequest.Tag.Key(), a.Tag.Key());
                     }
                 }
                ));
            // heartbeatactor
            AddBehavior(new Behavior<HeartBeatActor, HeartBeatAction>
                (
                (a, h) =>
                {
                    foreach (var worker in fWorkers.Where(w => w.Value.TTL < fTTL))
                    {
                        worker.Key.SendMessage((IActor)this, BrokerAction.Hearbeat);
                    }
                    fTTL++;
                    LogString("Heart Beat Signal, Request Processed {0}", fRequestProcessed);
                }
                ));
            // heartbeat answer
            AddBehavior(new Behavior<IActor, WorkerReadyState>(
                (a, w) =>
                {
                    var workerState = fWorkers[a];
                    workerState.TTL = fTTL;
                    LogString("Answer To HeartBeat from Worker {0}", a.Tag.Key());
                }));
            // start heart beat
            SendMessage(BrokerAction.Start);
        }
    }

}
