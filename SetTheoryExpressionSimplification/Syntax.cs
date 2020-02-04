using SetTheory.Structs;
using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace SetTheory
{
    public static class Syntax
    {
        static TokenizerBuilder<TokenType> SyntaxBuilder { get; } = new TokenizerBuilder<TokenType>()
            .Ignore(Span.WhiteSpace)
            .Match(Character.In('A', 'B', 'C', 'D', 'E'), TokenType.Set)
            .Match(Character.In('∪', '+', '∨', '|'), TokenType.Union)
            .Match(Character.In('∩', '*', '∧', '&'), TokenType.Intersection)
            .Match(Character.In('\\', '-'), TokenType.Difference)
            .Match(Character.In('△', '⊖'), TokenType.SymmetricDifference)
            .Match(Character.EqualTo('!'), TokenType.PrefixNegation)
            .Match(Character.EqualTo('\''), TokenType.PostfixNegation)
            .Match(Character.EqualTo('('), TokenType.LParen)
            .Match(Character.EqualTo(')'), TokenType.RParen)
            .Match(Character.In('U', '1'), TokenType.UniverseSet)
            .Match(Character.In('O', '0', '∅'), TokenType.EmptySet);

        public static Tokenizer<TokenType> Main { get; } = SyntaxBuilder.Build();

        public static Tokenizer<TokenType> WithVariables { get; } =
            SyntaxBuilder
            .Match(Span.Regex("_[A-E]"), TokenType.Variable)
            .Build();
    }
}