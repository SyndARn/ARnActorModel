using System.Collections.Generic;

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
        private readonly List<IEventSource<T>> _eventSources = new List<IEventSource<T>>();
        public void Write(IEventSource<T> aT)
        {
            _eventSources.Add(aT);
        }
        public IEnumerable<IEventSource<T>> Load()
        {
            foreach (var item in _eventSources)
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
