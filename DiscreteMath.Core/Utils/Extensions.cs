using DiscreteMath.Core.Language;
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

        internal static bool IsParensType(this Expression expr) => expr.Type == typeof(Parens);
        internal static bool IsOperatorType(this Expression expr) => expr is Operation;
        internal static bool OnlyChildIsOperator(this Expression expr) => expr.Children.Length == 1 && IsOperatorType(expr.Children[0]);
        internal static bool HasValue(this Expression expr, string value) => expr.Value == value;
        internal static bool AnyChildHasValue(this Expression expr, string value) => expr.Children.Any(x => x.Value == value);
        internal static bool IsVariable(this Expression pattern) => pattern.Value.StartsWith("_");
        internal static bool HasChildren(this Expression pattern) => pattern.Children.Length != 0;
        internal static bool IsSameExpression(this Expression expr1, Expression expr2)
            => expr1.Type == expr2.Type && expr1.Value == expr2.Value;
    }
}