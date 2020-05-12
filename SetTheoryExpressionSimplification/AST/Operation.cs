using System.Diagnostics;
using System.Linq;

namespace SetTheory
{
    [DebuggerDisplay("{Debug}")]
    public class Operation : Expression
    {
        public Operation(string value, params Expression[] children)
        {
            Value = value;
            Children = children;
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Copy(bool copyId = false)
            => copyId
                ? new Operation(Value, Children.Select(x => x.Copy(copyId)).ToArray()) { Id = Id }
                : new Operation(Value, Children.Select(x => x.Copy(copyId)).ToArray());

        public override string ToString() => $"{string.Join<Expression>($" {Value} ", Children)}";

        public string Debug => $"({string.Join<Expression>($" {Value} ", Children)})";
    }
}