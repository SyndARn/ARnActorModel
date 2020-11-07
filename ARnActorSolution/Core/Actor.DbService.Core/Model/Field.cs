using System;

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
}
