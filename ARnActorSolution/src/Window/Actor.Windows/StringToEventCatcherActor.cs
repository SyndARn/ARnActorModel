using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using System.Windows.Forms;

namespace Actor.Windows
{

    /// <summary>
    /// catch a C# event and publish it to a winform control
    /// </summary>
    public class EventCatcherActor<T> : ActionActor
    {

        EventHandler<T> fEvent = null;
        Control fControl = null;


        public void SetEvent(Control control, EventHandler<T> anEvent)
        {
            fEvent = anEvent;
            fControl = control;
            SendAction(DoWait);
        }

        private void DoWait()
        {
            var retval = ReceiveAsync(t => { return t is T; }).Result;
            if ((fEvent != null) && (fControl != null))
            {
                fControl.Invoke(fEvent, this, retval);
            }
        }

    }

    public class StringToEventCatcherActor : EventCatcherActor<string>
    {
    }

}
