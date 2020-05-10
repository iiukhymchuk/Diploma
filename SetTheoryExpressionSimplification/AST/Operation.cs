using System.Linq;

namespace SetTheory
{
    public class Operation : Expression
    {
        public Operation(string value, params Expression[] children)
        {
            Value = value;
            Children = children;
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Copy() => new Operation(Value, Children.Select(x => x.Copy()).ToArray());
        public override string ToString() => string.Join<Expression>($" {Value} ", Children);
    }
}