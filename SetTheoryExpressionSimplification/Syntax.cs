using SetTheory.Structs;
using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace SetTheory
{
    public class Syntax
    {
        readonly TokenizerBuilder<TokenType> syntaxBuilder;

        public Syntax(ISettings settings)
        {
            syntaxBuilder = CreateSyntaxBuilder(settings, new DefaultSettings());
        }

        TokenizerBuilder<TokenType> CreateSyntaxBuilder(ISettings settings, ISettings defaultSettings) =>
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

        public Tokenizer<TokenType> GetTokenizer => syntaxBuilder.Build();

        public Tokenizer<TokenType> GetTokenizerWithVariables =>
            syntaxBuilder
            .Match(Span.Regex("_[A-E]"), TokenType.Variable)
            .Build();
    }
}