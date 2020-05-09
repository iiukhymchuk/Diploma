namespace SetTheory
{
    public class Set : Expression
    {
        public Set(string value, Expression[] children)
            : base(value, children)
        {
            Value = value;
            Children = children;
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Copy() => Create<Set>(this);
        public override string ToString() => Value;
    }
}