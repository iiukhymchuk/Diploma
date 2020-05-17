using DiscreteMath.Core.Language;
using DiscreteMath.Core.Structs;
using Superpower;

namespace DiscreteMath.Core.Pipeline
{
    public class Rule
    {
        static readonly Tokenizer<TokenType> tokenizer;
        static readonly Grammar grammar;

        static Rule()
        {
            var settings = new DefaultSettings();
            tokenizer = new SyntaxWithVariables(settings).GetTokenizer();
            grammar = new Grammar(settings);
        }

        public Rule(string rule, string description)
        {
            var array = rule.Split('=', 2);
            var patternIn = Parse(array[0]);
            var patternOut = Parse(array[1]);

            PatternIn = patternIn;
            PatternOut = patternOut;
            Description = description;
        }

        public Expression PatternIn { get; }
        public Expression PatternOut { get; }
        public string Description { get; }

        static Expression Parse(string pattern)
        {
            var tokens = tokenizer.TryTokenize(pattern);
            var result = grammar.BuildTree(tokens.Value);
            return result.Value.Children[0];
        }
    }
}