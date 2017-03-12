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
            LinkedActor.SendMessage(typeof(ITrait<T>), aMessage);
        }

        private void ApplySetData(ITrait<T> aTrait,T aT)
        {
            fData = aT;
        }
    }

    public class ActorWithTrait : BaseActor, ITrait<string>
    {
        public ITrait<string> TraitService { get; set; }
        public ActorWithTrait() : base()
        {
        }

        public void SetData(string aMessage)
        {
            TraitService.SetData(aMessage);
        }
    }
}
