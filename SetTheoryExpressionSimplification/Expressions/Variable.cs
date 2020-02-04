namespace SetTheory.Expressions
{
    public class Variable : Expression
    {
        public Variable(string value, Expression[] children)
            : base(value, children)
        {
            Value = value;
            Children = children;
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Copy() => Create<Variable>(this);
        public override string ToString() => Value;
    }
}
