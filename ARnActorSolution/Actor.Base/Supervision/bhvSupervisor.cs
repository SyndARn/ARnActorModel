using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base.Supervision
{

    public interface ISupervisedActor
    {
        actActor Factory(actTag previousTag);
    }

    public class actSupervisedActor : actActor, ISupervisedActor
    {

        public actSupervisedActor() : base()
        {

        }

        protected actSupervisedActor(actTag previousTag) : base(previousTag)
        {

        }

        actActor ISupervisedActor.Factory(actTag previousTag)
        {
            actActor newActor = new actSupervisedActor(previousTag);
            return newActor;
        }
    }

    public class bhvSupervisor : Behaviors
    {
        public enum SupervisorAction { Register, Unregister, Alarm}

        private List<IActor> fSupervised = new List<IActor>();

        public bhvSupervisor() : base()
        {
            this.AddBehavior(new bhvBehavior<Tuple<bhvSupervisor.SupervisorAction,IActor>>(
                DoSupervision
                )) ;
        }

        private void DoSupervision(Tuple<bhvSupervisor.SupervisorAction,IActor> msg)
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
                case SupervisorAction.Alarm:
                    {
                        // how to relaunch this actor ?
                        break;
                    }
            }
        }
    }
}
