using System;
using System.Diagnostics;

namespace SetTheory
{
    [DebuggerDisplay("{Debug}")]
    public class Set : Expression
    {
        public Set(string value)
        {
            Value = value;
            Children = Array.Empty<Expression>();
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Copy(bool copyId = false)
            => new Set(Value);

        public override string ToString() => Value;
        public override string Debug => Value;
    }
}