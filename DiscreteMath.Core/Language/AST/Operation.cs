using System.Linq;

namespace DiscreteMath.Core.Language
{
    public abstract class Operation : Expression
    {
        public Operation(string value, params Expression[] children)
        {
            Value = value;
            Children = children;
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }
        public override string ToString() => $"{string.Join<Expression>($" {Value} ", Children)}";
        public override string Debug => $"({string.Join<string>($" {Value} ", Children.Select(x => x.Debug))})";
    }
}