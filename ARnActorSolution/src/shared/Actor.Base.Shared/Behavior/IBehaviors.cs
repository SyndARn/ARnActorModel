using System.Collections.Generic;

namespace Actor.Base
{
    public interface IBehaviors
    {
        IActor LinkedActor { get; }

        IBehaviors AddBehavior(IBehavior aBehavior);
        IEnumerable<IBehavior> AllBehaviors();
        IBehaviors BecomeBehavior(IBehavior aBehavior);
        bool FindBehavior(IBehavior aBehavior);
        void LinkToActor(IActor anActor);
        IBehaviors RemoveBehavior(IBehavior aBehavior);
    }
}