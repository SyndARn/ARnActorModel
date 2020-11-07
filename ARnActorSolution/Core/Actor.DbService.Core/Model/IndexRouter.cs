using System.Collections.Generic;
using Actor.Base;
using Actor.Util;
using System;
using System.Linq;
using System.IO;

namespace Actor.DbService.Core.Model
{
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
}
