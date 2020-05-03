using System.Collections.Generic;
using Actor.Base;
using Actor.Util;
using System;
using System.Linq;

namespace Actor.DbService.Core.Model
{
    public class Field
    {
        public string Name { get; set; }
        public string Value
        {
            get; set;
        }
        public DataFolder DataFolder
        {
            get ;
            set ; 
        }
        public string Uuid { get { return DataFolder.Uuid; } }
    }

    public class DataFolder : BaseActor
    {
        public string Uuid { get; private set; } = Guid.NewGuid().ToString();
        public string Source { get; private set; }
        private readonly List<Field> _fields = new List<Field>();
        public DataFolder(string source) : base()
        {            
            Source = source;
            Become(BehaviorAttributeBuilder.BuildFromAttributes(this).ToArray());
        }

        public void GetIndexNames(IActor actor)
        {
            this.SendMessage(actor);
        }

        [Behavior]
        protected void DoGetIndexNames(IActor actor)
        {
            actor.SendMessage(_fields.Select(f => f.Name));
        }

        private IEnumerable<string> DoParse(string source)
        {
            return source.Split(' ');
        }

        public void Parse(IndexRouter indexRouter)
        {
            foreach(var s in DoParse(Source))
            {
                        Field fieldName = new Field { DataFolder = this, Name = "Word", Value = s };
                        Field fieldSyllabe = new Field { DataFolder = this, Name = "Syllabe", Value = "1" };
                        Field fieldRime = new Field { DataFolder = this, Name = "Rime", Value = s };
                        Field fieldRich = new Field { DataFolder = this, Name = "Rich", Value = "1" };
                        _fields.AddRange(new[] { fieldName, fieldSyllabe, fieldRime, fieldRich });
                        indexRouter.AddField(fieldName);
                        indexRouter.AddField(fieldSyllabe);
                        indexRouter.AddField(fieldRime);
                        indexRouter.AddField(fieldRich);
            }
        }
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

        [Behavior]
        private void DoProcessQuery(Response response)
        {
            foreach(var index in _nameIndexes.Values)
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
                fields.Add(field);
                return;
            }
            _valueFields[field.Value] = new List<Field> { field };
        }

    }

    public interface IQuery
    {
        void Launch(IActor asker, IndexRouter router);
        Dictionary<string, string> Parameters { get; }
        bool FilterIndex(Index index);
        IEnumerable<Field> FilterValue(Dictionary<string, List<Field>> dico);
        string Uuid { get; }
    }

    public abstract class Query : IQuery
    {
        protected string QueryName { get; private set; }
        public Dictionary<string, string> Parameters { get; private set; } = new Dictionary<string, string>();
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
        public QueryByIndexEqualValue(string index, string value) : base()
        {
            Parameters["Index"] = index;
            Parameters["Value"] = value;
            Parameters["Op"] = "Equal";
        }

        public override bool FilterIndex(Index index)
        {
            return index.Name == Parameters["Index"] ;
        }

        public override IEnumerable<Field> FilterValue(Dictionary<string, List<Field>> dico)
        {
            if (dico.TryGetValue(Parameters["Value"], out List<Field> fields))
            {
                return fields;
            }
            return null;
        }
    }

    public class QueryByIndexContainsValue : Query
    {
        public QueryByIndexContainsValue(string index, string value) : base()
        {
            Parameters["Index"] = index;
            Parameters["Value"] = value;
            Parameters["Op"] = "Contains";
        }

        public override bool FilterIndex(Index index)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Field> FilterValue(Dictionary<string, List<Field>> dico)
        {
            throw new NotImplementedException();
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
        public IndexRouter Router { get; private set;}
        public IActor Asker { get; private set; }
        public IQuery Query { get; private set; }
    }
}
