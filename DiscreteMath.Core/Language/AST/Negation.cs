using System;

namespace DiscreteMath.Core.Language
{
    public class Negation : Expression
    {
        readonly bool isPrefixNegation;

        public Negation(string value, Expression child, bool isPrefixNegation = false)
        {
            this.isPrefixNegation = isPrefixNegation;
            Value = value;
            Children = new[] { child };
            Id = Guid.NewGuid();
        }

        Negation(string value, Expression child, bool isPrefixNegation, Guid id)
        {
            this.isPrefixNegation = isPrefixNegation;
            Value = value;
            Children = new[] { child };
            Id = id;
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Clone() => new Negation(Value, Children[0].Clone(), isPrefixNegation, Id);
        public override Expression Copy() => new Negation(Value, Children[0].Copy(), isPrefixNegation);
        public override string ToString() => $"{(isPrefixNegation ? Value + Children[0].ToString() : Children[0].ToString() + Value)}";
    }
}