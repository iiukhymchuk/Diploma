namespace SetTheory
{
    public class Tree : Expression
    {
        public Tree(string value, Expression child)
        {
            Value = value;
            Children = new[] { child };
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Copy(bool copyId = false)
            => new Tree(Value, Children[0].Copy(copyId));

        public override string ToString() => $"{Children[0]} ";
    }
}