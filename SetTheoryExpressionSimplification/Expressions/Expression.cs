using System;

namespace SetTheory.Expressions
{
    public abstract class Expression
    {
        // hint derived classes to implement this ctor
        protected Expression(string value, Expression[] children) { }

        public Type Type => GetType();
        public abstract string Value { get; }
        public abstract Expression[] Children { get; set; }

        public static Expression Create<TExpression>(Expression expression)
            where TExpression : Expression
        {
            return Create<TExpression>(expression.Value, expression.Children);
        }

        public static Expression Create<TExpression>(string value, params Expression[] children)
            where TExpression : Expression
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value), $"Default string value 'null' for parameter '{nameof(value)}' is not exeptable.");

            children ??= Array.Empty<Expression>();

            return (TExpression)Activator.CreateInstance(typeof(TExpression), value, children);
        }

        public abstract override string ToString();
        public abstract Expression Copy();
    }
}