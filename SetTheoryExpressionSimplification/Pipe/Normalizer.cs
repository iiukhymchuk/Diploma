using System.Linq;

namespace SetTheory
{
    class Normalizer
    {
        readonly string union;
        readonly string intersection;

        public Normalizer(ISettings settings)
        {
            var defaultSettings = new DefaultSettings();
            union = settings.Union ?? defaultSettings.Union;
            intersection = settings.Intersection ?? defaultSettings.Intersection;
        }

        internal Result<Substitution> Normalize(Expression expr)
        {
            var copy = expr.Copy();
            RemoveRedundantParens(copy);
            //expr.DFSPostOrder(x => x = CombineSameIntersections(x));
            //expr.DFSPostOrder(x => x = CombineSameUnions(x));
            //expr.DFSPostOrder(x => x = OrderByValue(x));

            if (Expression.ExprEquals(expr, copy))
                return Result<Substitution>.Empty();

            return new Result<Substitution>(
                new Substitution
                {
                    Expression = copy,
                    Description = "Simplification"
                });
        }

        void RemoveRedundantParens(Expression current)
        {
            if (current.Type == typeof(Tree) && IsParens(current.Children[0]))
                current.Children = current.Children[0].Children;

            var changeChildren = false;
            foreach (var child in current.Children)
            {
                RemoveRedundantParens(child);

                if (ParensMayBeOmited(child))
                    changeChildren = true;
            }

            if (changeChildren)
                current.Children = current.Children.Select(x => ParensMayBeOmited(x) ? x.Children[0] : x).ToArray();

            bool ParensMayBeOmited(Expression expr) => IsParens(expr) && !ChildIsOperator(expr);
        }

        //Expression CombineSameIntersections(Expression current)
        //{
        //    if (HasValue(current, intersection) && ChildrenHasValue(current, intersection))
        //        current.Children = current.Children
        //            .SelectMany(x => HasValue(x, intersection) ? x.Children : new[] { x })
        //            .ToArray();
        //    return current;
        //}

        //Expression CombineSameUnions(Expression current)
        //{
        //    if (HasValue(current, union) && ChildrenHasValue(current, union))
        //        current.Children = current.Children
        //            .SelectMany(x => HasValue(x, union) ? x.Children : new[] { x })
        //            .ToArray();
        //    return current;
        //}

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