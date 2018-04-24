using System;
using System.Collections.Generic;
using System.Text;

namespace Actor.Base
{
    public interface ITrait<T>
    {
        void SetData(T aMessage);
    }

    public class Trait<T> : Behavior<ITrait<T>,T>, ITrait<T>
    {
        private T fData;
        public Trait() : base()
        {
            Pattern = DefaultPattern();
            Apply = ApplySetData ;
        }

        public void SetData(T aMessage)
        {
            LinkedActor.SendMessage((ITrait<T>)this, aMessage);
        }

        private void ApplySetData(ITrait<T> aTrait,T aT)
        {
            fData = aT;
        }
    }

    public class ActorWithTrait<T> : BaseActor, ITrait<T>
    {
        private ITrait<T> _traitService;
        public ITrait<T> TraitService { get => _traitService;
            set
            {
                var bhv = value as IBehavior;
                _traitService = value;
                this.Become(bhv);
            }
        }
        public ActorWithTrait() : base()
        {
        }

        public void SetData(T aMessage)
        {
            TraitService.SetData(aMessage);
        }
    }
}
