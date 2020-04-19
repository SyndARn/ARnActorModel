using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    public class SupervisorBehavior : Behaviors
    {
        private readonly List<ISupervisedActor> _supervised = new List<ISupervisedActor>();

        public SupervisorBehavior() : base()
        {
            this.AddBehavior(new Behavior<SupervisorAction, ISupervisedActor>(
                DoSupervision
                )) ;
        }

        private void DoSupervision(SupervisorAction action, ISupervisedActor actor)
        {
            switch(action)
            {
                case SupervisorAction.Register:
                    {
                        _supervised.Add(actor);
                        break;
                    }
                case SupervisorAction.Unregister:
                    {
                        _supervised.Remove(actor);
                        break;
                    }
                case SupervisorAction.Respawn:
                    {
                        // how to relaunch this actor ?
                        _supervised.Remove(actor);
                        // create actor
                        var newactor = actor.Respawn();
                        _supervised.Add(newactor);
                        break;
                    }
            }
        }
    }
}
