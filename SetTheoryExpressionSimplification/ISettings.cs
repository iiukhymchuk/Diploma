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
    }
}