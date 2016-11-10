using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Actor.Service
{

    public interface IPersistentService<T>
    {
        void Write(IEventSource<T> aT);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        IEnumerable<IEventSource<T>> Load();
    }

    public class MemoizePersistentService<T> : IPersistentService<T>
    {
        private List<IEventSource<T>> fList = new List<IEventSource<T>>();
        public void Write(IEventSource<T> aT)
        {
            fList.Add(aT);
        }
        public IEnumerable<IEventSource<T>> Load()
        {
            foreach (var item in fList)
            {
                yield return item;
            }
        }
    }

    //public class LogPersistentService<T> : IPersistentService<T>
    //{
    //    private Stream fStream;
    //    public LogPersistentService(Stream aStream)
    //    {
    //        fStream = aStream;
    //    }

    //    public IEnumerable<IEventSource<T>> Load()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Write(IEventSource<T> aT)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
