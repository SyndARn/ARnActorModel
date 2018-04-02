using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;

namespace Actor.Util
{
    public class Consumer<T> : FsmActor<string, Work<T>>
    {
        public Consumer() : base()
        {
            var bhv = new FsmBehaviors<string, Work<T>>();

            bhv
                .AddRule("SleepState", null, t => t.SendMessage(this), "BusyState")
                .AddRule("BusyState", null, t => Buffer.SendMessage(this), "SleepState")
                .AddBehavior(new Behavior<Work<T>>(
                    t =>
                    {
                        this.SendMessage(GetCurrentState().Result(), t);
                    }));

            Become(bhv);
        }

        public Buffer<T> Buffer { get; set; }
    }
}
