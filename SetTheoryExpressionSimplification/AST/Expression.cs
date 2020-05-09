using System;
using System.Linq;

namespace SetTheory
{
    public abstract class Expression
    {
        public Type Type => GetType();
        public abstract string Value { get; }
        public abstract Expression[] Children { get; set; }

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