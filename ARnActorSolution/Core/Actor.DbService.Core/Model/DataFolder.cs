using System.Collections.Generic;
using Actor.Base;
using Actor.Util;
using System;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Collections.Concurrent;
using System.Xml.Schema;
using System.Threading.Tasks;
using System.Net;

namespace Actor.DbService.Core.Model
{
    public class Field
    {
        public string Name { get; set; }
        public string Value
        {
            get; set;
        }

        public string Keyword
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

        public override string ToString()
        {
            return $"Field : Name {Name} Keyword {Keyword} Value {Value} Uuid {Uuid}";
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

    public static class Functer
    {
        static char[] voyels = new[] { 'a', 'e', 'i', 'o', 'u' };
        static CultureInfo frenchCulture = new CultureInfo("fr-FR");

        static public List<string> voyelPatterns = new List<String>
        {
            "a","e","i","o","u",
            "an","en","in","on","un",
            "au","ai",
            "eu",
            "ou",
            "oui",
            "bru",
            "ti",
            "deu",
            "qua",
            "tre","troi",
            "roi"

        };

        static public string voyel = "aeiou";

        static public bool IsVoyel(char c)
        {
            return voyel.Contains(c);
        }




        public static readonly Func<string, DataFolder, IEnumerable<Field>> RimeFuncter = (s, folder) =>
        {
            var word = s.Trim().Trim(new char[] { ',', ';', ':' });
            var lowerWord = word.ToLower(frenchCulture);
            var parse = lowerWord;
            var syllabeCount = 0;
            var rime = "";

            while (parse != "")
            {
                // find a first pattern
                var patts = voyelPatterns.Select(v => parse.StartsWith(v) ? v : "").OrderByDescending(v => v.Length).FirstOrDefault();
                if ((patts != null) && (patts != ""))
                {
                    rime = rime + patts;
                    syllabeCount++;
                    parse = parse.Substring(patts.Length,parse.Length-patts.Length);
                }
                else
                {
                    break;
                }
            }
            
           

            List<Field> fields = new List<Field>
            {
            new Field { DataFolder = folder, Name = "Word", Value = word, Keyword = word },
            new Field { DataFolder = folder, Name = "Syllabe", Value =$"{syllabeCount}", Keyword = word },
            new Field { DataFolder = folder, Name = "Rime", Value = rime, Keyword = word  },
            new Field { DataFolder = folder, Name = "Rich", Value = $"{syllabeCount}", Keyword = word },
            new Field { DataFolder = folder, Name = "TimeStamp", Value = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff",CultureInfo.InvariantCulture), Keyword = word },
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

        public void AddFields(IEnumerable<Field> fields)
        {
            this.SendMessage(fields);
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
            foreach (var index in _nameIndexes.Values)
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
            response.Query.SendMessage(response, (IEnumerable<Index>)_nameIndexes.Values);
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

        [Behavior]
        private void AddFieldsBehavior(IEnumerable<Field> fields)
        {
            foreach (var field in fields)
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
            this.SendMessage(writer, future);
            return future.Result();
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
            foreach (var value in _valueFields.Values)
            {
                response.Query.SendMessage(response, value.AsEnumerable());
            }
        }

        [Behavior]
        protected void DoAddField(Field field)
        {
            if (_valueFields.TryGetValue(field.Value, out List<Field> fields))
            {
                if (!fields.Any(f => f.Uuid == field.Uuid && f.Keyword == field.Keyword))
                {
                    fields.Add(field);
                }
                return;
            }
            _valueFields[field.Value] = new List<Field> { field };
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

        public Query()
        {
            QueryName = ToString();
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
            var collector = new CollectionActor<Field>();
            var asker = new BaseActor(new Behavior<string, IEnumerable<Field>>
                (
                (s, fs) =>
                {
                    foreach (var item in fs)
                    {
                        collector.Add(item);
                    }
                }
                ));
            var response = new Response(router, asker, this);
            Become(new Behavior<Response>(r => DoProcessQuery(r)));
            SendMessage(response);
            return collector.AsEnumerable();
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
            Become(new Behavior<Response, IEnumerable<Index>>(
                (r, idxs) =>
                {
                    foreach (var i in idxs)
                    {
                        if (i.Name == Index)
                        {
                            i.SendMessage(r);
                        }
                    }
                }));
            AddBehavior(new Behavior<Response, IEnumerable<Field>>(
                (r, fields) =>
                {
                    if (fields.Any(f => f.Value == Value))
                    {
                        r.Asker.SendMessage(Uuid, fields.Where(f => f.Value == Value));
                    }
                }));
            response.Router.SendMessage(response);
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
            Become(new Behavior<Response, IEnumerable<Index>>(
                (r, idxs) =>
                {
                    foreach (var idx in idxs)
                    {
                        if (idx.Name == Index)
                        {
                            idx.SendMessage(r);
                        }
                    }
                }));
            AddBehavior(new Behavior<Response, IEnumerable<Field>>(
                (r, fields) =>
                {
                    r.Asker.SendMessage(Uuid, fields);
                }));
            response.Router.SendMessage(response);
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
            Become(new Behavior<Response, IEnumerable<Index>>(
                (r, idxs) =>
                {
                    foreach (var idx in idxs)
                    {
                        if (idx.Name == Index)
                        {
                            idx.SendMessage(r);
                        }
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
