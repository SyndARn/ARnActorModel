using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{
    public enum CrudeAction { Select, Insert, Update, Delete } ;

    public class actCrudeActor<T> : actActor
    {
        public actCrudeActor()
            : base()
        {
            Become(new bhvCrudeBehavior<T>());
        }

        public void Select(T aT)
        {
            SendMessage(Tuple.Create(CrudeAction.Select, aT));
        }

        public T Insert()
        {
            SendMessage(Tuple.Create(CrudeAction.Insert, default(T)));
            var retVal = Receive(t => { return true; }).Result;
            return retVal == null ? default(T) : (T)retVal;
        }

        public void Delete()
        {
            SendMessage(Tuple.Create(CrudeAction.Delete,default(T))) ;
        }

        public void Update()
        {
            SendMessage(Tuple.Create(CrudeAction.Update,default(T))) ;
        }
    }

    public class bhvCrudeBehavior<T> : bhvBehavior<Tuple<CrudeAction, T>>
    {
         private T fValue;

         public bhvCrudeBehavior()
            : base()
        {
            fValue = default(T);
        }

        public void SelectValue(T msg)
        {
            fValue = msg;
        }

        public void InsertValue()
        {
            LinkedActor.SendMessage(fValue);
        }

        public void UpdateValue()
        {

        }

        public void DeleteValue()
        {
        }

    }
}
