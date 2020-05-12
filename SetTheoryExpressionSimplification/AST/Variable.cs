using System;

namespace SetTheory
{
    public class Variable : Expression
    {
        public Variable(string value)
        {
            Value = value;
            Children = Array.Empty<Expression>();
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Copy(bool copyId = false)
            => copyId
                ? new Variable(Value) { Id = Id }
                : new Variable(Value);

        public override string ToString() => Value;
    }
}
