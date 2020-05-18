using DiscreteMath.Core.Language;
using DiscreteMath.Core.Structs;
using DiscreteMath.Core.Utils;
using System.Collections.Generic;

namespace DiscreteMath.Core.Pipeline
{
    public class Processor
    {
        public static Result<List<SimplificationDescription>> Process(string input, ISettings settings)
        {
            var combinedSettings = settings.Combine(new DefaultSettings());
            var rules = new Rules();

            IProvideTokenizer syntax = new Syntax(combinedSettings); // inject
            var tokenizer = syntax.GetTokenizer();
            var tokensResult = tokenizer.TryTokenize(input);
            if (!tokensResult.HasValue)
            {
                var errorIndex = GetErrorIndex(tokensResult.ErrorPosition);
                var token = tokensResult.Remainder.First(1).ToStringValue();
                // use localized user error messages
                return new Result<List<SimplificationDescription>>($"Syntax error for input '{token}'", errorIndex, token);
            }

            var grammar = new Grammar(combinedSettings); // inject
            var parseResult = grammar.BuildTree(tokensResult.Value);
            if (!parseResult.HasValue)
            {
                var errorIndex = GetErrorIndex(parseResult.ErrorPosition);
                var token = parseResult.Remainder.ConsumeToken().Value.ToStringValue();
                // use localized user error messages
                return new Result<List<SimplificationDescription>>($"Syntax error for input '{token}'", errorIndex, token);
            }

            // work with errors
            var interpreter = new Interpreter(new PatternMatcher(rules.GetRules()), new Normalizer(combinedSettings));
            var result = interpreter.Interpretate(parseResult.Value);
            return new Result<List<SimplificationDescription>>(result);
        }

        static int GetErrorIndex(Superpower.Model.Position position)
        {
            return position.HasValue && position.Line == 1
                ? position.Column - 1
                : -1;
        }
    }
}