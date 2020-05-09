namespace SetTheory
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
        string UniverseSign { get; }
        string EmptySetSign { get; }
        bool IsPrefixNegation { get; }
        string PostfixNegation { get; }
        string PrefixNegation { get; }
        string Union { get; }
        string Intersection { get; }
        string Difference { get; }
        string SymmetricDifference { get; }
    }
}