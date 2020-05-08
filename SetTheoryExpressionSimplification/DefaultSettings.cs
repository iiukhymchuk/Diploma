namespace SetTheory
{
    public class DefaultSettings : ISettings
    {
        public char[] Sets { get; } = new[] { 'A', 'B', 'C', 'D', 'E' };
        public char[] Unions { get; } = new[] { '∪', '+', '∨', '|' };
        public char[] Intersections { get; } = new[] { '∩', '*', '∧', '&' };
        public char[] Differences { get; } = new[] { '\\', '-' };
        public char[] SymmetricDifferences { get; } = new[] { '△', '⊖' };
        public char[] UniverseSets { get; } = new[] { 'U', '1' };
        public char[] EmptySets { get; } = new[] { 'O', '0', '∅' };
        public char[] PrefixNegations { get; } = new[] { '!' };
        public char[] PostfixNegations { get; } = new[] { '\'' };
        public char[] LParens { get; } = new[] { '(' };
        public char[] RParens { get; } = new[] { ')' };
    }
}