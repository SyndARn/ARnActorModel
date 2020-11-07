using System.Collections.Generic;
using Actor.Base;
using System.Linq;

namespace Actor.DbService.Core.Model
{
    public class QueryByIndex : Query
    {
        internal string Index { get; private set; }
        public QueryByIndex(string index) : base()
        {
            Index = index;
        }

        protected override void DoProcessQuery(Response response)
        {
            AddBehavior(new Behavior<Response, IEnumerable<Index>>(
                (r, idxs) =>
                {
                    TotalMsg = idxs.Count();
                    if (TotalMsg == 0)
                    {
                        StopQuery();
                    }
                    foreach (var idx in idxs)
                    {
                        idx.ProcessQuery(r, f => true);
                    }
                }));
            AddBehavior(new Behavior<Response, IEnumerable<Field>>(
                (r, fields) =>
                {
                    r.Asker.SendMessage(Uuid, fields);
                }));
            response.Router.ProcessQuery(response, i => i.Name == Index);
        }

    }
}
