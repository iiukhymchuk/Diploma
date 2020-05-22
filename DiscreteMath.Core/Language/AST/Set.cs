using System;

namespace DiscreteMath.Core.Language
{
    public class Set : Expression
    {
        public Set(string value)
        {
            Value = value;
            Children = Array.Empty<Expression>();
            Id = Guid.NewGuid();
        }

        Set(string value, Guid id)
        {
            Value = value;
            Children = Array.Empty<Expression>();
            Id = id;
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Clone() => new Set(Value, Id);
        public override Expression Copy() => new Set(Value);

        public override string ToString() => Value;
    }
}