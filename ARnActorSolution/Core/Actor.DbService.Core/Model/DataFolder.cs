using System.Collections.Generic;
using Actor.Base;
using Actor.Util;
using System;
using System.Linq;

namespace Actor.DbService.Core.Model
{

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
}
