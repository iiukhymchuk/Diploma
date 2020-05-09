using Superpower.Model;
using System.Collections.Generic;

namespace SetTheory
{
    public class Pipeline
    {
        public static Result<List<SimplificationDescription>> Process(string input)
        {
            var settings = new DefaultSettings(); // get settings from outside, not default
            var rules = new Rules();

            IProvideTokenizer syntax = new Syntax(settings); // inject
            var tokenizer = syntax.GetTokenizer();
            var tokensResult = tokenizer.TryTokenize(input);
            if (!tokensResult.HasValue)
            {
                var errorIndex = GetErrorIndex(tokensResult.ErrorPosition);
                var token = tokensResult.Remainder.First(1).ToStringValue();
                // use localized user error messages
                return new Result<List<SimplificationDescription>>($"Syntax error for input '{token}'", errorIndex, token);
            }

            var grammar = new Grammar(settings); // inject
            var parseResult = grammar.BuildTree(tokensResult.Value);
            if (!parseResult.HasValue)
            {
                var errorIndex = GetErrorIndex(parseResult.ErrorPosition);
                var token = parseResult.Remainder.ConsumeToken().Value.ToStringValue();
                // use localized user error messages
                return new Result<List<SimplificationDescription>>($"Syntax error for input '{token}'", errorIndex, token);
            }

            // work with errors
            var interpreter = new Interpreter(new PatternMatcher(rules.GetRules()), new Normalizer(rules.GetNormalizationRules()));
            var result = interpreter.Interpretate((Tree)parseResult.Value);
            return new Result<List<SimplificationDescription>>(result);
        }

        static int GetErrorIndex(Position position)
        {
            return position.HasValue && position.Line == 1
                ? position.Column - 1
                : -1;
        }
    }
}