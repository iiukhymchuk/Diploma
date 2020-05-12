namespace SetTheory
{
    public class Parens : Expression
    {
        public Parens(Expression child)
        {
            Children = new[] { child };
        }

        public override string Value { get; } = "()";
        public override Expression[] Children { get; set; }

        public override Expression Copy() => new Parens(Children[0].Copy());
        public override string ToString() => $"({Children[0]})";
    }
}