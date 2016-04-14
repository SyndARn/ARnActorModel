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
    public class PersistentActor<T> : BaseActor 
    {
        private T fCurrentValue;
        public PersistentActor(IPersistentService<T> service, string actorName) : base()
        {
            var act = DirectoryActor.GetDirectory().GetActorByName(actorName);
            if (act == null)
            {
                DirectoryActor.GetDirectory().Register(this, actorName);
            }
            Become(new PersistentLoadBehavior<T>(service));
            AddBehavior(new PersistentWriteBehavior<T>(service));
            AddBehavior(new Behavior<T>(Transact));
            AddBehavior(new Behavior<IActor>(a => a.SendMessage<T>(fCurrentValue)));
        }

        private void Transact(T aT)
        {
            this.SendMessage(PersistentCommand.Write, aT);
            fCurrentValue = aT;
        }

        public Future<T> GetCurrent()
        {
            var future = new Future<T>();
            this.SendMessage<IActor>(future);
            return future;
        }

        public void Reload()
        {
            var future = new Future<IEnumerable<T>>();
            this.SendMessage(PersistentCommand.Load, (IActor)future);
            fCurrentValue = future.Result().Last();
        }
    }

    public enum PersistentCommand { Write, Load} 

    public interface IPersistentService<T>
    {
        void Write(T aT);
        IEnumerable<T> Load();
    }

    public class MemoizePersistentService<T> : IPersistentService<T>
    {
        private List<T> fList = new List<T>();
        public void Write(T aT)
        {
            fList.Add(aT);
        }
        public IEnumerable<T> Load()
        {
            foreach(var item in fList)
            {
                yield return item;
            }
        }
    }

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
            sender.SendMessage(load);
        }
    }

    public class PersistentWriteBehavior<T> : Behavior<PersistentCommand,T>
    {
        IPersistentService<T> fService;
        public PersistentWriteBehavior(IPersistentService<T> service) : base()
        {
            fService = service;
            Apply = DoApply;
            Pattern = DoPattern;
        }

        private bool DoPattern(PersistentCommand command, T aT)
        {
            return command == PersistentCommand.Write;
        }

        private void DoApply(PersistentCommand command, T aT)
        {
            fService.Write(aT);
        }
    }

}
