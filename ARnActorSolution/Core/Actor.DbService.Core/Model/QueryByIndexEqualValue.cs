using System.Collections.Generic;
using Actor.Base;
using System.Linq;

namespace Actor.DbService.Core.Model
{
    public class QueryByIndexEqualValue : Query
    {
        internal string Index { get; private set; }
        internal string Value { get; private set; }
        public QueryByIndexEqualValue(string index, string value) : base()
        {
            Index = index;
            Value = value;
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
                        idx.ProcessQuery(r, f => f.Value == Value);
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
