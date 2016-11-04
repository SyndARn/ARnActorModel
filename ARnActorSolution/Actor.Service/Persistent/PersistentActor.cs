using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Server;
using Actor.Util;

namespace Actor.Service
{

    public interface IEventSource<T>
    {
        T Apply(T aState);
    }

    public class EventSource<T> : IEventSource<T>
    {
        public T Data { get; protected set; }
        public EventSource() : base()
        {
        }
        public virtual T Apply(T aState)
        {
            Data = aState;
            return Data;
        }
    }

    public class PersistentActor<T> : BaseActor 
    {
        private T fCurrentState ;
        public PersistentActor(IPersistentService<T> service, string actorName) : base()
        {
            var act = DirectoryActor.GetDirectory().GetActorByName(actorName);
            if (act == null)
            {
                DirectoryActor.GetDirectory().Register(this, actorName);
            }
            Become(new PersistentLoadBehavior<T>(service));
            AddBehavior(new PersistentWriteBehavior<T>(service));
            AddBehavior(new Behavior<IEventSource<T>>(Transact));
            AddBehavior(new Behavior<PersistentCommand,IActor>(                
                (c,a) => c == PersistentCommand.GetCurrent,
                (c,a) => a.SendMessage<T>(fCurrentState)));
        }

        private void Transact(IEventSource<T> anEvent)
        {
            this.SendMessage(PersistentCommand.Write, anEvent);
            fCurrentState = anEvent.Apply(fCurrentState);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public Future<T> GetCurrent()
        {
            var future = new Future<T>();
            this.SendMessage(PersistentCommand.GetCurrent,(IActor)future);
            return future;
        }

        public void Reload()
        {
            var future = new Future<IEnumerable<IEventSource<T>>>();
            this.SendMessage(PersistentCommand.Load, (IActor)future);
            fCurrentState = default(T);
            foreach(var item in future.Result())
            {
                fCurrentState = item.Apply(fCurrentState);
            }            
        }
    }

    public enum PersistentCommand { Write, Load, GetCurrent} 

    public class PersistentLoadBehavior<T> : Behavior<PersistentCommand, IActor>
    {
        IPersistentService<T> fService;
        public PersistentLoadBehavior(IPersistentService<T> service) : base()
        {
            fService = service;
            Apply = DoApply;
            Pattern = DoPattern;
        }

        private bool DoPattern(PersistentCommand command, IActor sender)
        {
            return command == PersistentCommand.Load;
        }

        private void DoApply(PersistentCommand command, IActor sender)
        {
            var load = fService.Load();
            sender.SendMessage<IEnumerable<IEventSource<T>>>(load);
        }
    }

    public class PersistentWriteBehavior<T> : Behavior<PersistentCommand,IEventSource<T>>
    {
        IPersistentService<T> fService;
        public PersistentWriteBehavior(IPersistentService<T> service) : base()
        {
            fService = service;
            Apply = DoApply;
            Pattern = DoPattern;
        }

        private bool DoPattern(PersistentCommand command, IEventSource<T> aT)
        {
            return command == PersistentCommand.Write;
        }

        private void DoApply(PersistentCommand command, IEventSource<T> aT)
        {
            fService.Write(aT);
        }
    }

}
