namespace SetTheory
{
    public class BinaryOperation : Expression
    {
        public BinaryOperation(string value, params Expression[] children)
        {
            Value = value;
            Children = children;
        }

        public override string Value { get; }
        public override Expression[] Children { get; set; }

        public override Expression Copy() => new BinaryOperation(Value, Children);
        public override string ToString() => $"{Children[0]} {Value} {Children[1]}";
    }
}