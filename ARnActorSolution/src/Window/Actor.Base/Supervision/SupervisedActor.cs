using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{

    public class SupervisedActor : BaseActor, ISupervisedActor
    {
        public virtual ISupervisedActor Respawn()
        {
            ISupervisedActor actor = new SupervisedActor(this.Tag);
            return actor;
        }

        public SupervisedActor(ActorTag previousTag) : base(previousTag) => Become(new SupervisedBehavior());

        public SupervisedActor() : base() => Become(new SupervisedBehavior());
    }
}
