using DiscreteMath.Core.Language;
using DiscreteMath.Core.Structs;
using DiscreteMath.Core.Utils;
using System.Collections.Generic;
using System.Linq;

namespace DiscreteMath.Core.Pipeline
{
    class Normalizer
    {
        readonly string intersection;
        readonly string union;

        public Normalizer(ISettings settings)
        {
            ISettings defaultSettings = new DefaultSettings();
            intersection = settings.Intersection ?? defaultSettings.Intersection;
            union = settings.Union ?? defaultSettings.Union;
        }

        internal Result<Substitution> Normalize(Expression expr)
        {
            var copy = expr.Copy();

            var changed = false;
            var descriptions = new List<string>();

            if (RemoveRedundantParens(copy))
            {
                changed = true;
                descriptions.Add("Remove redundant parentheses");
            }
            if (CombineOperators(copy))
                changed = true;

            if (changed)
                return new Result<Substitution>(
                new Substitution
                {
                    ResultingExpression = copy,
                    Description = string.Join<string>($", ", descriptions.Distinct())
                });

            return Result<Substitution>.Empty();
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

        bool CombineOperators(Expression expr)
        {
            var combined = false;

            expr.DFSPostOrder(x => Combine(x, intersection));
            expr.DFSPostOrder(x => Combine(x, union));

            return combined;

            void Combine(Expression expr, string value)
            {
                if (!HasValue(expr, value))
                    return;

                if (expr.Children.Any(child => HasValue(child, value)))
                {
                    combined = true;
                    expr.Children = expr.Children
                        .SelectMany(x => HasValue(x, value) ? x.Children : new[] { x })
                        .ToArray();
                }
            }
        }

        bool IsParensType(Expression expr) => expr.Type == typeof(Parens);
        bool IsOperatorType(Expression expr) => expr is Operation;
        bool OnlyChildIsOperator(Expression expr) => expr.Children.Length == 1 && IsOperatorType(expr.Children[0]);
        bool HasValue(Expression expr, string value) => expr.Value == value;
        bool AnyChildHasValue(Expression expr, string value) => expr.Children.Any(x => x.Value == value);
    }
}