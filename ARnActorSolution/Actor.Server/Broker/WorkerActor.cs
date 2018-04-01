using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Server
{

    public abstract class WorkerActor<T> : BaseActor
    {
        private WorkerReadyState fState = WorkerReadyState.Idle;

        protected WorkerActor() : base()
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
