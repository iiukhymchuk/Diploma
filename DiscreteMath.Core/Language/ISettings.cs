using System.Linq;

namespace DiscreteMath.Core.Language
{
    public interface ISettings
    {
        char[] Sets { get; }
        char[] Unions { get; }
        char[] Intersections { get; }
        char[] Differences { get; }
        char[] SymmetricDifferences { get; }
        char[] PrefixNegations { get; }
        char[] PostfixNegations { get; }
        char[] LParens { get; }
        char[] RParens { get; }
        char[] UniverseSets { get; }
        char[] EmptySets { get; }
        bool IsPrefixNegation { get; }
        string UniverseSign => (UniverseSets is null || !UniverseSets.Any()) ? null : UniverseSets[0].ToString();
        string EmptySetSign => (EmptySets is null || !EmptySets.Any()) ? null : EmptySets[0].ToString();
        string PostfixNegation => (PostfixNegations is null || !PostfixNegations.Any()) ? null : PostfixNegations[0].ToString();
        string PrefixNegation => (PrefixNegations is null || !PrefixNegations.Any()) ? null : PrefixNegations[0].ToString();
        string Union => (Unions is null || !Unions.Any()) ? null : Unions[0].ToString();
        string Intersection => (Intersections is null || !Intersections.Any()) ? null : Intersections[0].ToString();
        string Difference => (Differences is null || !Differences.Any()) ? null : Differences[0].ToString();
        string SymmetricDifference => (SymmetricDifferences is null || !SymmetricDifferences.Any()) ? null : SymmetricDifferences[0].ToString();
    }
}