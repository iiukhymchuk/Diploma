using DiscreteMath.Core.Language;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscreteMath.Core.Utils
{
    static class Extensions
    {
        internal static ISettings Combine(this ISettings source, ISettings other)
        {
            return new Settings
            {
                Sets = source.Sets ?? other.Sets,
                Unions = source.Unions ?? other.Unions,
                Intersections = source.Intersections ?? other.Intersections,
                Differences = source.Differences ?? other.Differences,
                SymmetricDifferences = source.SymmetricDifferences ?? other.SymmetricDifferences,
                PrefixNegations = source.PrefixNegations ?? other.PrefixNegations,
                PostfixNegations = source.PostfixNegations ?? other.PostfixNegations,
                LParens = source.LParens ?? other.LParens,
                RParens = source.RParens ?? other.RParens,
                UniverseSets = source.UniverseSets ?? other.UniverseSets,
                EmptySets = source.EmptySets ?? other.EmptySets,
                IsPrefixNegation = source.IsPrefixNegation || other.IsPrefixNegation
            };
        }

        internal static IEnumerable<T> DistinctBy<T, T2>(this IEnumerable<T> source, Func<T, T2> selector)
        {
            return source
                .GroupBy(x => selector(x))
                .Select(x => x.First());
        }

        internal static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        internal static bool IsOperator(this Expression expr) =>
            expr.IsIntersection()
            || expr.IsUnion()
            || expr.IsDifference()
            || expr.IsNegation()
            || expr.IsSymmetricDifference();
        internal static bool IsIntersection(this Expression expr) => expr.Type == typeof(Intersection);
        internal static bool IsUnion(this Expression expr) => expr.Type == typeof(Union);
        internal static bool IsDifference(this Expression expr) => expr.Type == typeof(Difference);
        internal static bool IsNegation(this Expression expr) => expr.Type == typeof(Negation);
        internal static bool IsSymmetricDifference(this Expression expr) => expr.Type == typeof(SymmetricDifference);
        internal static bool IsVariable(this Expression pattern) => pattern.Value.StartsWith("_");
        internal static bool HasChildren(this Expression pattern) => pattern.Children.Length != 0;
        internal static bool IsSameExpression(this Expression expr1, Expression expr2)
            => expr1.Type == expr2.Type && expr1.Value == expr2.Value;
    }
}