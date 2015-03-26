using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ActorRing
{
    class actStringCatcher : actActionActor
    {

        public actStringCatcher()
            : base()
        {
        }

        EventHandler<string> fEvent = null;
        Control fControl = null;


        public void SetEvent(Control control,EventHandler<string> anEvent)
        {
            fEvent = anEvent;
            fControl = control;
            SendAction(DoWait);
        }

        private void DoWait()
        {
            var retval = Receive(t => { return t is string; }).Result;
            if ((fEvent !=null) && (fControl !=null))
            {
                fControl.Invoke(fEvent,this,retval.ToString()) ;
            }
        }


    }
}
