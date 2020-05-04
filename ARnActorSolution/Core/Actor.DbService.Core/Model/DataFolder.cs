using System.Collections.Generic;
using Actor.Base;
using Actor.Util;
using System;
using System.Linq;
using System.IO;
using System.Globalization;

namespace Actor.DbService.Core.Model
{
    public class Field
    {
        public string Name { get; set; }
        public string Value
        {
            get; set;
        }

        private DataFolder _dataFolder;
        public DataFolder DataFolder
        {
            get { return _dataFolder; }
            set { _dataFolder = value; Uuid = _dataFolder.Uuid; }
        }
        public string Uuid { get; set; }
    }

    public class DataFolder : BaseActor
    {
        public string Uuid { get; private set; } = Guid.NewGuid().ToString();
        public string Source { get; private set; }
        // private readonly List<Field> _fields = new List<Field>();
        public DataFolder(string source) : base()
        {
            Source = source;
            Become(BehaviorAttributeBuilder.BuildFromAttributes(this).ToArray());
        }

        private IEnumerable<string> DoParse(string source)
        {
            return source.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries) ;
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

    public static class Functer
    {
        public static readonly Func<string, DataFolder, IEnumerable<Field>> RimeFuncter = (s, folder) =>
        {
            List<Field> fields = new List<Field>
            {
            new Field { DataFolder = folder, Name = "Word", Value = s },
            new Field { DataFolder = folder, Name = "Syllabe", Value = "1" },
            new Field { DataFolder = folder, Name = "Rime", Value = s },
            new Field { DataFolder = folder, Name = "Rich", Value = "1" },
            new Field { DataFolder = folder, Name = "TimeStamp", Value = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff",CultureInfo.InvariantCulture) },
            };
            return fields;
        };
        public static readonly Func<string, DataFolder, IEnumerable<Field>> WordFuncter = (s, folder) =>
        {
            List<Field> fields = new List<Field>
            {
            new Field { DataFolder = folder, Name = "Word", Value = s },
            new Field { DataFolder = folder, Name = "Syllabe", Value = "1" },
            new Field { DataFolder = folder, Name = "TimeStamp", Value = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff",CultureInfo.InvariantCulture) },
            };
            return fields;
        };
    }

    public class IndexRouter : BaseActor
    {
        private readonly Dictionary<string, Index> _nameIndexes = new Dictionary<string, Index>();

        public IndexRouter() : base()
        {
            Become(BehaviorAttributeBuilder.BuildFromAttributes(this).ToArray()); ;
        }

        public void AddField(Field field)
        {
            this.SendMessage(field);
        }

        public void ProcessQuery(Response response)
        {
            this.SendMessage(response);
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
            foreach(var index in _nameIndexes.Values)
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
            // read name
            // read value
            // read folder
            while (!stream.EndOfStream)
            {
                var name = stream.ReadLine();
                var value = stream.ReadLine();
                var folder = stream.ReadLine();
                // process index
                Field field = new Field() { Name = name.Split('=').Last(), Uuid = folder.Split('=').Last(), Value = value.Split('=').Last() };
                AddFieldBehavior(field);
            }

        }

        [Behavior]
        private void DoProcessQuery(Response response)
        {
            foreach (var index in _nameIndexes.Values)
            {
                if (response.Query.FilterIndex(index))
                {
                    index.ProcessQuery(response);
                }
            }
        }

        [Behavior]
        private void AddFieldBehavior(Field field)
        {
            if (_nameIndexes.TryGetValue(field.Name, out Index index))
            {
                index.SendMessage(field);
                return;
            }
            Index newIndex = new Index(field.Name);
            _nameIndexes.Add(field.Name, newIndex);
            newIndex.SendMessage(field);
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
        private readonly Dictionary<string, List<Field>> _valueFields = new Dictionary<string, List<Field>>();

        public void ProcessQuery(Response response)
        {
            SendMessage(response);
        }

        public void AddField(Field field)
        {
            SendMessage(field);
        }

        public bool SaveToStream(StreamWriter writer)
        {
            var future = new Future<bool>();
            this.SendMessage(writer,future);
            return future.Result();
        }

        public bool LoadFromStream(StreamReader reader)
        {
            SendMessage(reader);
            return true;
        }

        [Behavior]
        private void DoLoadStream(StreamReader stream)
        {
        }

        [Behavior]
        private void DoSaveStream(StreamWriter stream, IActor future)
        {
            // stream.WriteLine($"FieldName = {Name}");
            foreach (var fields in _valueFields)
            {
                foreach (var field in fields.Value)
                {
                    stream.WriteLine($"Name={field.Name}");
                    stream.WriteLine($"Value={field.Value}");
                    stream.WriteLine($"Folder={field.Uuid}");
                }
            }
            future.SendMessage(true);
        }

        [Behavior]
        private void StreamAllValue(IActor actor)
        {
            foreach (List<Field> value in _valueFields.Values)
            {
                foreach (Field field in value)
                {
                    actor.SendMessage(field.Value);
                }
            }
        }

        [Behavior]
        protected void DoProcessQuery(Response response)
        {
            var fields = response.Query.FilterValue(_valueFields);
            response.Asker.SendMessage(response.Query.Uuid, fields);
        }

        [Behavior]
        protected void DoAddField(Field field)
        {
            if (_valueFields.TryGetValue(field.Value, out List<Field> fields))
            {
                if (!fields.Any(f => f.Uuid == field.Uuid))
                {
                    fields.Add(field);
                }
                return;
            }
            _valueFields[field.Value] = new List<Field> { field };
        }

    }

    public interface IQuery
    {
        void Launch(IActor asker, IndexRouter router);
        bool FilterIndex(Index index);
        IEnumerable<Field> FilterValue(Dictionary<string, List<Field>> dico);
        string Uuid { get; }
    }

    public abstract class Query : IQuery
    {
        protected string QueryName { get; private set; }
        public string Uuid { get; } = Guid.NewGuid().ToString();

        public Query()
        {
            QueryName = ToString();
        }

        public void Launch(IActor asker, IndexRouter router)
        {
            CheckArg.Actor(router);
            var response = new Response(router, asker, this);
            router.ProcessQuery(response);
        }

        public abstract IEnumerable<Field> FilterValue(Dictionary<string, List<Field>> dico);
        public abstract bool FilterIndex(Index index);

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

        public override bool FilterIndex(Index index)
        {
            return index.Name == Index;
        }

        public override IEnumerable<Field> FilterValue(Dictionary<string, List<Field>> dico)
        {
            if (dico.TryGetValue(Value, out List<Field> fields))
            {
                return fields;
            }
            return null;
        }
    }

    public class QueryByIndex : Query
    {
        internal string Index { get; private set; }
        public QueryByIndex(string index) : base()
        {
            Index = index;
        }

        public override bool FilterIndex(Index index)
        {
            return index.Name == Index;
        }

        public override IEnumerable<Field> FilterValue(Dictionary<string, List<Field>> dico)
        {
            foreach(var value in dico.Values)
            {
                foreach(var v in value)
                    yield return v;
            }
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

        public override bool FilterIndex(Index index)
        {
            return true;
        }

        public override IEnumerable<Field> FilterValue(Dictionary<string, List<Field>> dico)
        {
            if (dico.TryGetValue(Value, out List<Field> fields))
            {
                return fields;
            }
            return null;
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
