using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;

namespace Actor.Util
{

    // Data Proxy

    public class DataProxy : BaseActor, IData
    {
        private DataObject fObject;
        public DataProxy() : base()
        {
            fObject = new DataObject();
            var behaviorStore = new Behavior<string>(t => fObject.Store(t));
            var behaviorRetrieve = new Behavior<IFuture<string>>(t => t.SendMessage(fObject.Retrieve()));
            Become(behaviorStore, behaviorRetrieve);
        }
        public void Store(string aData)
        {
            this.SendMessage(aData);
        }
        public string Retrieve()
        {
            IFuture<string> future = new Future<string>();
            this.SendMessage(future);
            return future.Result();
        }
    }

    public interface IData
    {
        void Store(string aData);
        string Retrieve();
    }

    public class DataObject : IData
    {
        private string fData;
        public DataObject()
        {
        }
        public void Store(string aData)
        {
            fData = aData;
        }
        public string Retrieve()
        {
            return fData;
        }
    }
}
