using System.Collections.Generic;
using Actor.Base;
using Actor.Util;
using System;

namespace Actor.DbService.Core.Model
{
    // document : list of fields
    // index : list of document from a field
    // parsing create fields
    // index each fields
    // store index in document

    public class Field
    {
        public string Name { get; set; }
        public string Value
        {
            get; set;
        }
        public Document Doc
        {
            get; set;
        }
    }

    public class Document : BaseActor
    {
        private readonly string _uuid;
        private string _source;
        private readonly List<Field> _fields;
        // Field
        public Document() : base()
        {

        }

        [Behavior]
        public void Parse(string source, IndexRouter indexRouter)
        {
            _source = source;
            StringParserActor parser = new StringParserActor();
            parser.ParseString(this, _source);
            Become(new Behavior<IActor, string>
                (
                (a, s) =>
                {
                    Field fieldName = new Field { Doc = this, Name = "Word", Value = s };
                    Field fieldSyllabe = new Field { Doc = this, Name = "Syllabe", Value = "1" };
                    Field fieldRime = new Field { Doc = this, Name = "Rime", Value = s };
                    Field fieldRich = new Field { Doc = this, Name = "Rich", Value = "1" };
                    indexRouter.SendMessage(fieldName);
                    indexRouter.SendMessage(fieldSyllabe);
                    indexRouter.SendMessage(fieldRime);
                    indexRouter.SendMessage(fieldRich);
                }
                ));
        }
    }

    public class IndexRouter : BaseActor
    {
        private readonly Dictionary<string, Index> _nameIndexes = new Dictionary<string, Index>();

        public IndexRouter() : base()
        {
            Become(new ActionBehavior<Field>());
            Become(new ActionBehavior<IActor, string, string>());
        }

        public void AddField(Field field)
        {
            this.SendMessage<Action<Field>,Field>(AddFieldBehavior, field);
        }

        public void QueryByName(IActor asker, string queryType, string name)
        {
            this.SendMessage<Action<IActor,string,string>,IActor,string,string>(QueryByNameBehavior, asker, queryType, name);
        }

        public void QueryByNameValue(IActor asker, string queryType, string name, string value)
        {
            this.SendMessage<Action<IActor,string,string,string>, IActor, string, string, string>(QueryByNameValueBehavior, asker, queryType, name, value);
        }

        public void QueryByValue(IActor asker, string value)
        {
            this.SendMessage<Action<IActor,string>,IActor,string>(QueryByValueBehavior, asker, value);
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
        private void QueryByNameBehavior(IActor asker, string queryType, string name)
        {
            if (_nameIndexes.TryGetValue(name, out Index index))
            {
                index.SendMessage("StreamAllValue", asker);
            }
        }

        [Behavior]
        private void QueryByNameValueBehavior(IActor asker, string queryType, string name, string value)
        {
            if (_nameIndexes.TryGetValue(name, out Index index))
            {
                index.SendMessage("EqualValue", value, asker);
            }
        }

        [Behavior]
        private void QueryByValueBehavior(IActor asker, string value)
        {
            foreach (Index index in _nameIndexes.Values)
            {
                index.SendMessage(asker, "EqualValue", value);
            }
        }


    }

    public class Index : BaseActor
    {

        public Index(string name) : base()
        {
            _name = name;
        }

        private string _name { get; set; }
        private readonly Dictionary<string, List<Field>> _valueFields = new Dictionary<string, List<Field>>();

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
        private void EqualValue(IActor asker, string value)
        {
            if (_valueFields.TryGetValue(value, out List<Field> fields))
            {
                foreach (Field field in fields)
                {
                    asker.SendMessage(field.Doc);

                }
            }
        }

        [Behavior]
        private void AddField(Field field)
        {
            if (_valueFields.TryGetValue(field.Name, out List<Field> fields))
            {
                fields.Add(field);
                return;
            }
            _valueFields[field.Name] = new List<Field> { field };
        }

    }

    public class Query : BaseActor
    {
        private readonly IndexRouter _indexRouter;

        public Query(IndexRouter indexRouter) : base()
        {
            _indexRouter = indexRouter;
        }

        public void QueryByFieldValueNameValue(IActor asker, string name, string value)
        {
            this.SendMessage(nameof(QueryByFieldNameValueBehavior),asker, name, value);
        }

        public void QueryByName(IActor asker, string name)
        {
            this.SendMessage(nameof(QueryByName), asker, name);
        }

        public void QueryByValue(IActor asker, string value)
        {
            this.SendMessage(nameof(QueryByValue), asker, value);
        }

        [Behavior]
        private void QueryByFieldNameValueBehavior(string name, string value, IActor asker)
        {
            _indexRouter.SendMessage("EqualValue", name, value, asker);
        }

        [Behavior]
        private void QueryByName(string name, IActor asker)
        {
            _indexRouter.SendMessage("StreamAllValue", name, asker);
        }

        [Behavior]
        private void QueryByValue(string value, IActor asker)
        {
            _indexRouter.SendMessage("StreamAllName", value, asker);
        }
    }
    // read a document
    // source
    // field name value document
    // add them to index Field

}
