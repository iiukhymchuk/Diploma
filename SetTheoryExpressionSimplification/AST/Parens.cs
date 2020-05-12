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

        public override Expression Copy(bool copyId = false)
            => copyId
                ? new Parens(Children[0].Copy(copyId)) { Id = Id }
                : new Parens(Children[0].Copy(copyId));

        public override string ToString() => $"({Children[0]})";
    }
}