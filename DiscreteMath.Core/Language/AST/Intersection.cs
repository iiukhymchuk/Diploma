using System;
using System.Linq;

namespace DiscreteMath.Core.Language
{
    public class Intersection : Expression
    {
        public Intersection(string value, params Expression[] children)
        {
            Value = value;
            Children = children;
            Id = Guid.NewGuid();
        }

        Intersection(string value, Expression[] children, Guid id)
        {
            Value = value;
            Children = children;
            Id = id;
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Clone() => new Intersection(Value, Children.Select(x => x.Clone()).ToArray(), Id);
        public override Expression Copy() => new Intersection(Value, Children.Select(x => x.Copy()).ToArray());
        public override string ToString() => $"({string.Join<string>($" {Value} ", Children.Select(x => x.ToString()))})";
    }
}