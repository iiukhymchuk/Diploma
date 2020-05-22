using System;
using System.Linq;

namespace DiscreteMath.Core.Language
{
    public class SymmetricDifference : Expression
    {
        public SymmetricDifference(string value, params Expression[] children)
        {
            Value = value;
            Children = children;
            Id = Guid.NewGuid();
        }

        SymmetricDifference(string value, Expression[] children, Guid id)
        {
            Value = value;
            Children = children;
            Id = id;
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Clone() => new SymmetricDifference(Value, Children.Select(x => x.Clone()).ToArray(), Id);
        public override Expression Copy() => new SymmetricDifference(Value, Children.Select(x => x.Copy()).ToArray());
        public override string ToString() => $"({string.Join<string>($" {Value} ", Children.Select(x => x.ToString()))})";
    }
}