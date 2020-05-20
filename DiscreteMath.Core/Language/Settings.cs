namespace DiscreteMath.Core.Language
{
    public class Settings : ISettings
    {
        public char[] Sets { get; set; }
        public char[] Unions { get; set; }
        public char[] Intersections { get; set; }
        public char[] Differences { get; set; }
        public char[] SymmetricDifferences { get; set; }
        public char[] PrefixNegations { get; set; }
        public char[] PostfixNegations { get; set; }
        public char[] LParens { get; set; }
        public char[] RParens { get; set; }
        public char[] UniverseSets { get; set; }
        public char[] EmptySets { get; set; }
        public bool IsPrefixNegation { get; set; }
    }
}