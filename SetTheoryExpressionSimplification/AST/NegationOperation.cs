namespace SetTheory
{
    public class NegationOperation : Expression
    {
        readonly bool isPrefixNegation;

        public NegationOperation(string value, Expression child, bool isPrefixNegation = false)
        {
            this.isPrefixNegation = isPrefixNegation;
            Value = value;
            Children = new[] { child };
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Copy(bool copyId = false)
            => copyId
                ? new NegationOperation(Value, Children[0].Copy(copyId), isPrefixNegation) { Id = Id }
                : new NegationOperation(Value, Children[0].Copy(copyId), isPrefixNegation);

        public override string ToString() => $"{(isPrefixNegation ? Value + Children[0] : Children[0] + Value)}";
    }
}