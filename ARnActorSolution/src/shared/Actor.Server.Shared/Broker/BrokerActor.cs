using System.Collections.Generic;
using System.Linq;
using Actor.Base;
using System.Globalization;

namespace Actor.Server
{
    /// <summary>
    /// <para>
    /// BrokerActor
    ///     Have an actor that upon receiving message will fan out this message to one or more of pre-registrered actor
    ///     BrokerActor are very usefull if they are registered, sending a message to the broker will forward it across the
    ///     shards.
    /// </para>
    /// <para>
    ///     a simple broker send a message to one of it workers, 
    ///     a more complex broker will take a different rout : one of free worker, a new spawned worker, etc ...
    /// </para>
    /// <para>
    ///     worker can be made simple, they just react to message
    ///     or complex : they become statefull and can alert the broker of their current status (alive, ready, busy ...)
    /// </para>
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
        public int TimeToLive { get; set; }
    }

    public class BrokerActor<T> : BaseActor
    {
        private readonly Dictionary<IActor, WorkerStatus> _workers = new Dictionary<IActor, WorkerStatus>();
        private readonly Dictionary<ActorTag, RequestStatus<T>> _requests = new Dictionary<ActorTag, RequestStatus<T>>();
        private int _lastWorkerUsed = 0;
        private int _TTL = 0;
        private int _requestProcessed = 0;
        public IActor Logger { get; set; } = new NullActor();

        public void RegisterWorker(IActor worker) => this.SendMessage(BrokerAction.RegisterWorker, worker);

        private IActor FindWorker()
        {
            if (_lastWorkerUsed < 0)
            {
                _lastWorkerUsed = 0;
            }
            else
            {
                if (_lastWorkerUsed >= _workers.Count)
                { _lastWorkerUsed = 0; }
            }
            var worker = _workers.Where(w => w.Value.State == WorkerReadyState.Idle).Skip(_lastWorkerUsed).FirstOrDefault();
            _lastWorkerUsed++;
            if (worker.Key == null)
            {
                worker = _workers.FirstOrDefault(w => w.Value.State == WorkerReadyState.Idle);
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
                     WorkerStatus workerStatus = new WorkerStatus
                     {
                         TimeToLive = 0,
                         State = WorkerReadyState.Idle
                     };
                     _workers.Add(a, workerStatus);
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
                     _workers.Remove(a);
                     LogString("Worker UnRegister", a.Tag.Key());
                 }
                );
        }

        public void LogString(string message) => Logger.SendMessage(message);

        public void LogString(string message, object arg0) => Logger.SendMessage(string.Format(CultureInfo.InvariantCulture, message, arg0));

        public void LogString(string message, object arg0, object arg1) => Logger.SendMessage(string.Format(CultureInfo.InvariantCulture, message, arg0, arg1));

        public void LogString(string message, object[] args) => Logger.SendMessage(string.Format(CultureInfo.InvariantCulture, message, args));

        private Behavior<T> BehaviorProcessClientRequest()
        {
            return new Behavior<T>(
                (t) =>
                {
                    var requestStatus = new RequestStatus<T>
                    {
                        Data = t,
                        State = RequestState.Unprocessed,
                        Tag = new ActorTag()
                    };
                    _requests[requestStatus.Tag] = requestStatus;

                    var worker = FindWorker();
                    if (worker != null)
                    {
                        _workers[worker].State = WorkerReadyState.Busy;
                        _workers[worker].TimeToLive = 0;
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
                     _workers[a].State = WorkerReadyState.Busy;
                     LogString("Worker {0} can't process request {1}", a.Tag.Key(), t.Key());
                     var worker = FindWorker();
                     if (worker != null)
                     {
                         _workers[worker].State = WorkerReadyState.Busy;
                         _workers[worker].TimeToLive = 0;
                         worker.SendMessage((IActor)this, t, _requests[t]);
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
                     _requestProcessed++;
                     _requests.Remove(t);
                     _workers[a].State = WorkerReadyState.Idle;
                     _workers[a].TimeToLive = _TTL;
                     LogString("Request {0} End on worker {1}", t.Key(), a.Tag.Key());
                     // find a request
                     RequestStatus<T> tagRequest = _requests.Values.FirstOrDefault(v => v.State == RequestState.Unprocessed);
                     if (tagRequest != null)
                     {
                         _workers[a].State = WorkerReadyState.Busy;
                         _workers[a].TimeToLive = 0;
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
                    foreach (var worker in _workers.Where(w => w.Value.TimeToLive < _TTL))
                    {
                        worker.Key.SendMessage((IActor)this, BrokerAction.Hearbeat);
                    }

                    _TTL++;
                    LogString("Heart Beat Signal, Request Processed {0}", _requestProcessed);
                }
                ));
            // heartbeat answer
            AddBehavior(new Behavior<IActor, WorkerReadyState>(
                (a, w) =>
                {
                    var workerState = _workers[a];
                    workerState.TimeToLive = _TTL;
                    LogString("Answer To HeartBeat from Worker {0}", a.Tag.Key());
                }));
            // start heart beat
            this.SendMessage(BrokerAction.Start);
        }
    }
}
