using System.Collections.Generic;
using System.Linq;

namespace SetTheory
{
    class Normalizer
    {
        readonly string intersection;
        readonly string union;

        public Normalizer(ISettings settings)
        {
            var defaultSettings = new DefaultSettings();
            intersection = settings.Intersection ?? defaultSettings.Intersection;
            union = settings.Union ?? defaultSettings.Union;
        }

        internal Result<Substitution> Normalize(Expression expr)
        {
            var copy = expr.Copy();

            var descriptions = new List<string>();

            if (RemoveRedundantParens(copy)) descriptions.Add("Remove redundant parentheses");
            CombineOperators(copy);
            //expr.DFSPostOrder(x => x = OrderByValue(x));

            if (Utils.ExprEquals(expr, copy))
                return Result<Substitution>.Empty();

            return new Result<Substitution>(
                new Substitution
                {
                    Expression = copy,
                    Description = string.Join<string>($", ", descriptions.Distinct())
        });
        }

        bool RemoveRedundantParens(Expression current)
        {
            bool changed = false;

            changed = changed || RemoveParensForWholeExpression(current);
            changed = changed || RemoveParensForNotOperators(current);
            changed = changed || RemoveParensForOperators(current);

            return changed;
        }

        bool RemoveParensForWholeExpression(Expression expr)
        {
            if (!HasRedundant(expr))
                return false;

            expr.Children = expr.Children[0].Children;
            return true;

            bool HasRedundant(Expression expr) => expr.Type == typeof(Tree) && IsParensType(expr.Children[0]);
        }

        bool RemoveParensForNotOperators(Expression expr)
        {
            var changed = false;
            expr.DFSPostOrder(x => Remove(x, ref changed));
            return changed;

            void Remove(Expression expr, ref bool chaged)
            {
                if (expr.Children.Any(child => HasRedundant(child)))
                {
                    expr.Children = expr.Children
                        .Select(x => HasRedundant(x) ? x.Children[0] : x)
                        .ToArray();
                    changed = true;
                }
                bool HasRedundant(Expression expr) => IsParensType(expr) && !OnlyChildIsOperator(expr);
            }
        }

        bool RemoveParensForOperators(Expression expr)
        {
            var changed = false;
            expr.DFSPostOrder(x => Remove(x, intersection, ref changed));
            expr.DFSPostOrder(x => Remove(x, union, ref changed));
            return changed;

            void Remove(Expression expr, string value, ref bool chaged)
            {
                if (!HasValue(expr, value))
                    return;

                if (expr.Children.Any(child => HasRedundant(child)))
                {
                    expr.Children = expr.Children
                        .Select(x => HasRedundant(x) ? x.Children[0] : x)
                        .ToArray();
                    changed = true;
                }
                bool HasRedundant(Expression expr) => IsParensType(expr) && AnyChildHasValue(expr, value);
            }
        }

        void CombineOperators(Expression expr)
        {
            expr.DFSPostOrder(x => Combine(x, intersection));
            expr.DFSPostOrder(x => Combine(x, union));

            void Combine(Expression expr, string value)
            {
                if (!HasValue(expr, value))
                    return;

                if (expr.Children.Any(child => MayBeCombined(child)))
                    expr.Children = expr.Children
                        .SelectMany(x => MayBeCombined(x) ? x.Children : new[] { x })
                        .ToArray();
                bool MayBeCombined(Expression expr) => HasValue(expr, value);
            }
        }

        bool IsParensType(Expression expr) => expr.Type == typeof(Parens);
        bool IsOperatorType(Expression expr) => expr.Type == typeof(Operation);
        bool OnlyChildIsOperator(Expression expr) => expr.Children.Length == 1 && IsOperatorType(expr.Children[0]);
        bool HasValue(Expression expr, string value) => expr.Value == value;
        bool AnyChildHasValue(Expression expr, string value) => expr.Children.Any(x => x.Value == value);
    }
}