using System.Collections.Generic;
using Actor.Base;
using Actor.Util;
using System;
using System.Linq;
using System.IO;

namespace Actor.DbService.Core.Model
{
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
}
