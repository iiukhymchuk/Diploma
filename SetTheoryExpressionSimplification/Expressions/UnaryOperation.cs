namespace SetTheory.Expressions
{
    public class UnaryOperation : Expression
    {
        public UnaryOperation(string value, Expression[] children)
            : base(value, children)
        {
            Value = value;
            Children = children;
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Copy() => Create<UnaryOperation>(this);
        public override string ToString() => $"{Children[0].ToString()}{Value}";
    }
}