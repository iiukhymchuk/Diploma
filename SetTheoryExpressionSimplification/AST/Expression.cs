using System;
using System.Linq;

namespace SetTheory
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

        public static bool ExprEquals(Expression first, Expression second)
        {
            if (first.Type != second.Type) return false;
            if (first.Value != second.Value) return false;
            if (first.Children.Length != second.Children.Length) return false;

            foreach (var two in first.Children.Zip(second.Children, (x, y) => (x, y)))
            {
                if (!ExprEquals(two.x, two.y))
                    return false;
            }
            return true;
        }

        public abstract Expression Copy();
        public abstract override string ToString();
    }
}