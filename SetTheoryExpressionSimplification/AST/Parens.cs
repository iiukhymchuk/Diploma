namespace SetTheory
{
    public class Parens : Expression
    {
        public Parens(string value, Expression child)
        {
            Value = value;
            Children = new[] { child };
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Copy() => new Parens(Value, Children[0].Copy());
        public override string ToString() => $"({Children[0]})";
    }
}