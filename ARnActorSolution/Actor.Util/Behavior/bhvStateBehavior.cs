using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{

    public class actStatefullActor<T> : actActor
    {
        public actStatefullActor() : base()
        {
            Become(new bhvStateBehavior<T>());
        }

        public void Set(T aT)
        {
            SendMessageTo(Tuple.Create(StateAction.Set,aT)) ;
        }

        public T Get()
        {
            SendMessageTo(Tuple.Create(StateAction.Get,default(T))) ;
            var retVal = Receive(t => {return true;}).Result ;
            return retVal == null ? default(T) : (T)retVal ;
        }
    }

    public enum StateAction { Set, Get } ;

    public class bhvStateBehavior<T> : bhvBehavior<Tuple<StateAction, T>>
    {
        private T fValue;

        public bhvStateBehavior() : base()
        {
            fValue = default(T);
        }

        public void SetValue(T msg)
        {
            fValue = msg ;
        }

        public void GetValue()
        {
            SendMessageTo(fValue, this.LinkedTo().LinkedActor);
        }
    }

}
