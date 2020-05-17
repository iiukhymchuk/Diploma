using System;
using System.Diagnostics;

namespace DiscreteMath.Core.Language
{
    [DebuggerDisplay("{Debug}")]
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
            => new Variable(Value);

        public override string ToString() => Value;
        public override string Debug => Value;
    }
}