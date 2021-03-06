﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Actor.Base;

namespace Actor.Server
{
    public class BrokerActor<T> : BaseActor
    {
        private readonly Dictionary<IActor, WorkerStatus> _workers = new Dictionary<IActor, WorkerStatus>();
        private readonly Dictionary<ActorTag, RequestStatus<T>> _requests = new Dictionary<ActorTag, RequestStatus<T>>();
        private int _lastWorkerUsed = 0;
        private int _ttl = 0;
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
                {
                    _lastWorkerUsed = 0;
                }
            }

            KeyValuePair<IActor, WorkerStatus> worker = _workers.Where(w => w.Value.State == WorkerReadyState.Idle).Skip(_lastWorkerUsed).FirstOrDefault();
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
                    HeartBeatActor actor = new HeartBeatActor(30000);
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
                    RequestStatus<T> requestStatus = new RequestStatus<T>
                    {
                        Data = t,
                        State = RequestState.Unprocessed,
                        Tag = new ActorTag()
                    };
                    _requests[requestStatus.Tag] = requestStatus;

                    IActor worker = FindWorker();
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
                     IActor worker = FindWorker();
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
                         _workers[a].TimeToLive = _ttl;
                         LogString("Request {0} End on worker {1}", t.Key(), a.Tag.Key());
                         // find a request
                         RequestStatus<T> tagRequest = _requests.Values.FirstOrDefault(v => v.State == RequestState.Unprocessed);
                         if (tagRequest == null)
                         {
                             return;
                         }

                         _workers[a].State = WorkerReadyState.Busy;
                         _workers[a].TimeToLive = 0;
                         tagRequest.State = RequestState.Running;
                         a.SendMessage((IActor)this, tagRequest.Tag, tagRequest.Data);
                         LogString("Processing Request {0} reusing worker {1}", tagRequest.Tag.Key(), a.Tag.Key());
                     }
                ));
            // heartbeatactor
            AddBehavior(new Behavior<HeartBeatActor, HeartBeatAction>
                (
                (a, h) =>
                {
                    foreach (KeyValuePair<IActor, WorkerStatus> worker in _workers.Where(w => w.Value.TimeToLive < _ttl))
                    {
                        worker.Key.SendMessage((IActor)this, BrokerAction.Hearbeat);
                    }

                    _ttl++;
                    LogString("Heart Beat Signal, Request Processed {0}", _requestProcessed);
                }
                ));
            // heartbeat answer
            AddBehavior(new Behavior<IActor, WorkerReadyState>(
                (a, w) =>
                {
                    WorkerStatus workerState = _workers[a];
                    workerState.TimeToLive = _ttl;
                    LogString("Answer To HeartBeat from Worker {0}", a.Tag.Key());
                }));
            // start heart beat
            SendMessage(BrokerAction.Start);
        }
    }
}
