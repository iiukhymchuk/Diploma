using System;

namespace SetTheory
{
    public class Set : Expression
    {
        public Set(string value)
        {
            Value = value;
            Children = Array.Empty<Expression>();
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Copy() => new Set(Value);
        public override string ToString() => Value;
    }
}