using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace SetTheory
{
    interface IProvideTokenizer
    {
        Tokenizer<TokenType> GetTokenizer();
    }

    class Syntax : IProvideTokenizer
    {
        readonly TokenizerBuilder<TokenType> syntaxBuilder;

        public Syntax(ISettings settings)
        {
            syntaxBuilder = new SyntaxBuilder(settings).Builder;
        }

        public Tokenizer<TokenType> GetTokenizer() => syntaxBuilder.Build();
    }

    class SyntaxWithVariables : IProvideTokenizer
    {
        readonly TokenizerBuilder<TokenType> syntaxBuilder;

        public SyntaxWithVariables(ISettings settings)
        {
            syntaxBuilder = new SyntaxBuilder(settings).Builder;
        }

        public Tokenizer<TokenType> GetTokenizer()
            => syntaxBuilder
                .Match(Span.Regex("_[A-E]"), TokenType.Variable)
                .Build();
    }

    class SyntaxBuilder
    {
        internal TokenizerBuilder<TokenType> Builder { get; }

        internal SyntaxBuilder(ISettings settings)
        {
            var defaultSettings = new DefaultSettings();

            Builder =
                new TokenizerBuilder<TokenType>()
                .Ignore(Span.WhiteSpace)
                .Match(Character.In(settings.Sets ?? defaultSettings.Sets), TokenType.Set)
                .Match(Character.In(settings.Unions ?? defaultSettings.Unions), TokenType.Union)
                .Match(Character.In(settings.Intersections ?? defaultSettings.Intersections), TokenType.Intersection)
                .Match(Character.In(settings.Differences ?? defaultSettings.Differences), TokenType.Difference)
                .Match(Character.In(settings.SymmetricDifferences ?? defaultSettings.SymmetricDifferences), TokenType.SymmetricDifference)
                .Match(Character.In(settings.PrefixNegations ?? defaultSettings.PrefixNegations), TokenType.PrefixNegation)
                .Match(Character.In(settings.PostfixNegations ?? defaultSettings.PostfixNegations), TokenType.PostfixNegation)
                .Match(Character.In(settings.LParens ?? defaultSettings.LParens), TokenType.LParen)
                .Match(Character.In(settings.RParens ?? defaultSettings.RParens), TokenType.RParen)
                .Match(Character.In(settings.UniverseSets ?? defaultSettings.UniverseSets), TokenType.UniverseSet)
                .Match(Character.In(settings.EmptySets ?? defaultSettings.EmptySets), TokenType.EmptySet);
        }
    }
}