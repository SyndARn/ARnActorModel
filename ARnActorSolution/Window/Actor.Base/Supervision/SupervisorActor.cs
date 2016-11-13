using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{

    public interface ISupervisedActor : IActor
    {
        ISupervisedActor Respawn();
    }


    public class SupervisedActor : BaseActor, ISupervisedActor
    {
        public virtual ISupervisedActor Respawn()
        {
            ISupervisedActor actor = new SupervisedActor(this.Tag);
            return actor;
        }
        public SupervisedActor(ActorTag previousTag) : base(previousTag)
        {
            Become(new SupervisedBehavior());
        }
        public SupervisedActor() : base()
        {
            Become(new SupervisedBehavior());
        }
    }

    public enum SupervisorAction { Register, Unregister, Respawn, Kill} ;

    public class SupervisedBehavior : Behavior<SupervisorAction>
    {
        public SupervisedBehavior()
        {
            Pattern =  t =>{return true;} ;
            Apply = t =>
             {
                 if (t.Equals(SupervisorAction.Kill))
                 {
                     LinkedActor.SendMessage(SystemMessage.NullBehavior);
                 }
             };
        }
    }

    public class SupervisorActor : BaseActor
    {
        public SupervisorActor() : base (new SupervisorBehavior())
        {

        }
    }

    public class SupervisorBehavior : Behaviors
    {

        private List<ISupervisedActor> fSupervised = new List<ISupervisedActor>();

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
                        fSupervised.Add(actor);
                        break; 
                    }
                case SupervisorAction.Unregister:
                    {
                        fSupervised.Remove(actor);
                        break;
                    }
                case SupervisorAction.Respawn:
                    {
                        // how to relaunch this actor ?
                        fSupervised.Remove(actor);
                        // create actor
                        var newactor = actor.Respawn();
                        fSupervised.Add(newactor);
                        break;
                    }
            }
        }
    }
}
