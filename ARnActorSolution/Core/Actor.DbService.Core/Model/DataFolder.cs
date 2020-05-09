using System.Collections.Generic;
using Actor.Base;
using Actor.Util;
using System;
using System.Linq;
using System.IO;
using System.Collections.Concurrent;
using System.Xml.Schema;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace Actor.DbService.Core.Model
{
    public class Field : IEquatable<Field>
    {
        public string FieldName { get; set; }
        public string Keyword
        {
            get; set;
        }
        public string Value
        {
            get; set;
        }

        public string Uuid { get; set; }

        public bool Equals(Field other)
        {
            if (other == null) return false;
            return (other.FieldName == FieldName) && (other.Keyword == Keyword) && (other.Uuid == Uuid) && (other.Value == Value);
        }

        public override string ToString()
        {
            return $"Field : Name {FieldName} Keyword {Keyword} Value {Value} Uuid {Uuid}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Field);
        }

        public override int GetHashCode()
        {
            return Uuid.GetHashCode(StringComparison.InvariantCulture);
        }
    }

    public class DataFolder : BaseActor
    {
        public string Uuid { get; private set; } = Guid.NewGuid().ToString();
        public string Source { get; private set; }

        public DataFolder(string source) : base()
        {
            Source = source;
            Become(BehaviorAttributeBuilder.BuildFromAttributes(this).ToArray());
        }

        private IEnumerable<string> DoParse(string source)
        {
            return source.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
        }

        public void Parse(IndexRouter indexRouter, Func<string, DataFolder, IEnumerable<Field>> fieldProducer)
        {
            foreach (var s in DoParse(Source))
            {
                foreach (var field in fieldProducer(s, this))
                {
                    indexRouter.AddField(field);
                }
            }
        }
    }

    public class IndexRouter : BaseActor
    {
        private readonly HashSet<Index> _nameIndexes = new HashSet<Index>();

        public IndexRouter() : base()
        {
            Become(BehaviorAttributeBuilder.BuildFromAttributes(this).ToArray()); ;
        }

        public void AddField(Field field)
        {
            this.SendMessage(field);
        }

        public void AddFields(IEnumerable<Field> fields)
        {
            this.SendMessage(fields);
        }


        public void ProcessQuery(Response response, Func<Index, bool> evaluator)
        {
            this.SendMessage(response, evaluator);
        }

        public void SaveToStream(StreamWriter stream)
        {
            SendMessage(stream);
        }

        public void LoadFromStream(StreamReader stream)
        {
            SendMessage(stream);
        }

        [Behavior]
        private void DoSaveToStream(StreamWriter stream)
        {
            foreach (var index in _nameIndexes)
            {
                if (!index.SaveToStream(stream))
                {
                    break;
                }
            }
        }

        [Behavior]
        private void DoLoadFromStream(StreamReader stream)
        {
            while (!stream.EndOfStream)
            {
                var s = stream.ReadLine();
                var sp = s.Split("|",4);
                string name="";
                string value="";
                string keyword="";
                string uuid="";
                foreach(var ikv in sp)
                {
                    var kv = ikv.Split("=",2);
                    switch (kv[0])
                    {
                        case "Name": name = kv[1]; break;
                        case "Value": value = kv[1]; break;
                        case "Keyword": keyword = kv[1]; break;
                        case "Uuid": uuid = kv[1]; break;
                    }
                }
                Field field = new Field() { FieldName = name, Uuid = uuid, Value = value, Keyword = keyword };
                AddFieldBehavior(field);
            }
        }

        [Behavior]
        private void DoProcessQuery(Response response, Func<Index, bool> evaluator)
        {
            response.Query.SendMessage(response, _nameIndexes.Where(k => evaluator(k)));
        }

        [Behavior]
        private void AddFieldBehavior(Field field)
        {
            var index = _nameIndexes.FirstOrDefault(f => f.Name == field.FieldName);
            if (index == null)
            {
                index = new Index(field.FieldName);
                _nameIndexes.Add(index);
            }
            index.SendMessage(field);
        }

        [Behavior]
        private void AddFieldsBehavior(IEnumerable<Field> fields)
        {
            foreach (var field in fields)
            {
                AddFieldBehavior(field);
            }
        }
    }

    public class Index : BaseActor
    {

        public Index(string name) : base()
        {
            Name = name;
            Become(BehaviorAttributeBuilder.BuildFromAttributes(this).ToArray());
        }

        public string Name { get; private set; }
        private readonly HashSet<Field> _valueFields = new HashSet<Field>();


        public void ProcessQuery(Response response, Func<Field, bool> evaluator)
        {
            this.SendMessage(response, evaluator, true); // don't touch
        }

        public void AddField(Field field)
        {
            SendMessage(field);
        }

        public bool SaveToStream(StreamWriter writer)
        {
            var future = new Future<bool>();
            this.SendMessage(writer, future);
            return future.Result();
        }

        [Behavior]
        private void DoSaveStream(StreamWriter stream, IActor future)
        {
            foreach (var field in _valueFields)
            {
                stream.WriteLine($"Name={field.FieldName}|Value={field.Value}|Keyword={field.Keyword}|Uuid={field.Uuid}");
            }
            future.SendMessage(true);
        }

        [Behavior]
        private void StreamAllValue(IActor actor)
        {
            actor.SendMessage(_valueFields.ToList().AsEnumerable());
        }

        [Behavior]
        protected void DoProcessQuery(Response response, Func<Field, bool> evaluator, bool value)
        {
            response.Query.SendMessage(response, _valueFields.Where(f => evaluator(f)));
        }

        [Behavior]
        protected void DoAddField(Field field)
        {
            if (!_valueFields.TryGetValue(field, out _))
            {
                _valueFields.Add(field);
            } else
            {
                Console.WriteLine($"found duplicate {field}");
            }
        }

    }

    public interface IQuery : IActor
    {
        string Uuid { get; }

        void Launch(IActor asker, IndexRouter router);
        IEnumerable<Field> Launch(IndexRouter router);
    }

    public abstract class Query : BaseActor, IQuery
    {
        protected string QueryName { get; private set; }
        public string Uuid { get; } = Guid.NewGuid().ToString();

        protected int TotalMsg { get; set; }
        protected int ReceivedMsg { get; set; }

        private int streamStart = 0;
        private int streamEnd = 0;

        public Query()
        {
            QueryName = ToString();
        }

        public void StopQuery()
        {
            Interlocked.Increment(ref streamStart);
            Interlocked.Increment(ref streamEnd);
        }

        public void Launch(IActor asker, IndexRouter router)
        {
            CheckArg.Actor(router);
            var response = new Response(router, asker, this);
            Become(new Behavior<Response>(r => DoProcessQuery(r)));
            SendMessage(response);
        }

        public IEnumerable<Field> Launch(IndexRouter router)
        {
            CheckArg.Actor(router);
            ConcurrentQueue<Field> bc = new ConcurrentQueue<Field>();
            var spin = new SpinWait();
            var asker = new BaseActor(new Behavior<string, IEnumerable<Field>>
                (
                (s, fs) =>
                {
                    foreach (var item in fs)
                    {
                        bc.Enqueue(item);
                    }
                    ReceivedMsg++;
                    if (ReceivedMsg == 1)
                    {
                        Interlocked.Increment(ref streamStart);
                    }
                    if (ReceivedMsg >= TotalMsg)
                    {
                        Interlocked.Increment(ref streamEnd);
                    }
                }
                ));
            var response = new Response(router, asker, this);
            Become(new Behavior<Response>(r => DoProcessQuery(r)));
            SendMessage(response);
            while (Interlocked.CompareExchange(ref streamStart, streamStart, 0) == 0)
            {
                spin.SpinOnce();
            }

            do
            {
                while (bc.TryDequeue(out Field field))
                {
                    yield return field;
                };
                spin.SpinOnce();
            }
            while (Interlocked.CompareExchange(ref streamEnd, streamEnd, 0) == 0);
        }

        protected abstract void DoProcessQuery(Response response);
    }

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

    public class Response
    {
        public Response(IndexRouter router, IActor asker, IQuery query)
        {
            Router = router;
            Asker = asker;
            Query = query;
        }
        public IndexRouter Router { get; private set; }
        public IActor Asker { get; private set; }
        public IQuery Query { get; private set; }
    }
}
