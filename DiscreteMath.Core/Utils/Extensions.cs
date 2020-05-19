using DiscreteMath.Core.Language;

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
    }
}