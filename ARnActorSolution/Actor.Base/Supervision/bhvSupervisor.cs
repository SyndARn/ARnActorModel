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


    public class actSupervisedActor : actActor, ISupervisedActor
    {
        public virtual ISupervisedActor Respawn()
        {
            ISupervisedActor actor = new actSupervisedActor(this.Tag);
            return actor;
        }
        public actSupervisedActor(actTag previousTag) : base(previousTag)
        {
            Become(new bhvSupervised());
        }
        public actSupervisedActor() : base()
        {
            Become(new bhvSupervised());
        }
    }

    public enum SupervisorAction { Register, Unregister, Respawn, Kill} ;

    public class bhvSupervised : bhvBehavior<SupervisorAction>
    {
        public bhvSupervised()
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

    public class actSupervisor : actActor
    {
        public actSupervisor() : base (new bhvSupervisor())
        {

        }
    }

    public class bhvSupervisor : Behaviors
    {

        private List<ISupervisedActor> fSupervised = new List<ISupervisedActor>();

        public bhvSupervisor() : base()
        {
            this.AddBehavior(new bhvBehavior<Tuple<SupervisorAction, ISupervisedActor>>(
                DoSupervision
                )) ;
        }

        private void DoSupervision(Tuple<SupervisorAction, ISupervisedActor> msg)
        {
            switch(msg.Item1)
            {
                case SupervisorAction.Register: 
                    {
                        fSupervised.Add(msg.Item2);
                        break; 
                    }
                case SupervisorAction.Unregister:
                    {
                        fSupervised.Remove(msg.Item2);
                        break;
                    }
                case SupervisorAction.Respawn:
                    {
                        // how to relaunch this actor ?
                        fSupervised.Remove(msg.Item2);
                        // create actor
                        var newactor = msg.Item2.Respawn();
                        fSupervised.Add(newactor);
                        break;
                    }
            }
        }
    }
}
