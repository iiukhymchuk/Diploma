using System;
using System.Linq;

namespace DiscreteMath.Core.Language
{
    public class Union : Expression
    {
        public Union(string value, params Expression[] children)
        {
            Value = value;
            Children = children;
            Id = Guid.NewGuid();
        }

        Union(string value, Expression[] children, Guid id)
        {
            Value = value;
            Children = children;
            Id = id;
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Clone() => new Union(Value, Children.Select(x => x.Clone()).ToArray(), Id);
        public override Expression Copy() => new Union(Value, Children.Select(x => x.Copy()).ToArray());
        public override string ToString() => $"({string.Join<string>($" {Value} ", Children.Select(x => x.ToString()))})";
    }
}