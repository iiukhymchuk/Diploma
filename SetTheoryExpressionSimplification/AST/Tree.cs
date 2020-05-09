namespace SetTheory
{
    public class Tree : Expression
    {
        public Tree(string value, Expression[] children)
            : base(value, children)
        {
            Value = value;
            Children = children;
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Copy() => Create<Tree>(this);
        public override string ToString() => $"{Children[0]}";
    }
}