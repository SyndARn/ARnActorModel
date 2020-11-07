using System.Collections.Generic;
using Actor.Base;
using System.Linq;

namespace Actor.DbService.Core.Model
{
    public class QueryByIndexContainsValue : Query
    {
        internal string Index { get; private set; }
        internal string Value { get; private set; }

        public QueryByIndexContainsValue(string index, string value) : base()
        {
            Index = index;
            Value = value;
        }

        protected override void DoProcessQuery(Response response)
        {
            AddBehavior(new Behavior<Response, IEnumerable<Index>>(
                (r, idxs) =>
                {
                    TotalMsg = idxs.Count(i => i.Name == Index);
                    if (TotalMsg == 0)
                    {
                        StopQuery();
                    }
                    foreach (var idx in idxs.Where(i => i.Name == Index))
                    {
                        idx.SendMessage(r);
                    }
                }));
            AddBehavior(new Behavior<Response, IEnumerable<Field>>(
                (r, fields) =>
                {
                    if (fields.Any(f => f.Value == Value))
                    {
                        r.Asker.SendMessage(Uuid, fields);
                    }
                }));
            response.Router.SendMessage(response);
        }
    }
}
