using System.Collections.Generic;
using Actor.Base;

namespace Actor.DbService.Core.Model
{
    public interface IQuery : IActor
    {
        string Uuid { get; }

        void Launch(IActor asker, IndexRouter router);
        IEnumerable<Field> Launch(IndexRouter router);
    }
}
