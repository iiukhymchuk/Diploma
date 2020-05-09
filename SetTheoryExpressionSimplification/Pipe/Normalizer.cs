using System;
using System.Collections.Generic;
using System.Linq;

namespace SetTheory
{
    class Normalizer
    {
        readonly List<Rule> rules;
        readonly DefaultSettings settings;

        public Normalizer(List<Rule> rules)
        {
            this.rules = rules;
            settings = new DefaultSettings();
        }

        internal Tree Normalize(Tree expr)
        {
            expr.DFSPostOrder(x => x = RemoveRedundantParens(x));
            expr.DFSPostOrder(x => x = CombineSameIntersections(x));
            expr.DFSPostOrder(x => x = CombineSameUnions(x));
            //expr.DFSPostOrder(x => x = OrderByValue(x));

            return expr;
        }

        Expression RemoveRedundantParens(Expression current)
        {
            if (IsParens(current) && !ChildIsOperator(current))
                return current.Children[0];
            return current;
        }

        Expression CombineSameIntersections(Expression current)
        {
            if (HasValue(current, settings.Intersection) && ChildrenHasValue(current, settings.Intersection))
                current.Children = current.Children
                    .SelectMany(x => HasValue(x, settings.Intersection) ? x.Children : new[] { x })
                    .ToArray();
            return current;
        }

        Expression CombineSameUnions(Expression current)
        {
            if (HasValue(current, settings.Union) && ChildrenHasValue(current, settings.Union))
                current.Children = current.Children
                    .SelectMany(x => HasValue(x, settings.Union) ? x.Children : new[] { x })
                    .ToArray();
            return current;
        }

        //Expression OrderByValue(Expression current)
        //{
        //    current.Children = current.Children.OrderBy(x => x.Value).ToArray();
        //    return current;
        //}

        bool IsParens(Expression expr) => expr.Type == typeof(Parens);
        bool IsOperator(Expression expr) => expr.Type == typeof(BinaryOperation);
        bool ChildIsOperator(Expression expr) => expr.Children.Length == 1 && IsOperator(expr.Children[0]);
        bool HasValue(Expression expr, string value) => expr.Value == value;
        bool ChildrenHasValue(Expression expr, string value) => expr.Children.Any(x => x.Value == value);
    }
}