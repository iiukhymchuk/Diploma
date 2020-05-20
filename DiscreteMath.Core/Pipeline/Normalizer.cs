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

        internal MyResult<Substitution> Normalize(Expression expr)
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
                return new MyResult<Substitution>(
                new Substitution
                {
                    ResultingExpression = copy,
                    Description = string.Join<string>($", ", descriptions.Distinct())
                });

            return MyResult<Substitution>.Empty();
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

            bool HasRedundant(Expression expr) => expr.Type == typeof(Tree) && expr.Children[0].IsParensType();
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
                bool HasRedundant(Expression expr) => expr.IsParensType() && !expr.OnlyChildIsOperator();
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
                if (!expr.HasValue(value))
                    return;

                if (expr.Children.Any(child => HasRedundant(child)))
                {
                    expr.Children = expr.Children
                        .Select(x => HasRedundant(x) ? x.Children[0] : x)
                        .ToArray();
                    changed = true;
                }
                bool HasRedundant(Expression expr) => expr.IsParensType() && expr.AnyChildHasValue(value);
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
                if (!expr.HasValue(value))
                    return;

                if (expr.Children.Any(child => child.HasValue(value)))
                {
                    combined = true;
                    expr.Children = expr.Children
                        .SelectMany(x => x.HasValue(value) ? x.Children : new[] { x })
                        .ToArray();
                }
            }
        }
    }
}