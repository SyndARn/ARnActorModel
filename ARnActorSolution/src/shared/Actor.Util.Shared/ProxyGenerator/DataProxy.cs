using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;

namespace Actor.Util
{

    // Data Proxy

    public class DataProxy : BaseActor, IData
    {
        private readonly DataObject _object;
        public DataProxy() : base()
        {
            _object = new DataObject();
            var behaviorStore = new Behavior<string>(t => _object.Store(t));
            var behaviorRetrieve = new Behavior<IFuture<string>>((IFuture<string> t) => t.SendMessage(_object.Retrieve()));
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
        private string _data;
        public DataObject()
        {
        }
        public void Store(string aData)
        {
            _data = aData;
        }
        public string Retrieve()
        {
            return _data;
        }
    }
}
