using Superpower.Model;
using System.Collections.Generic;

namespace SetTheory
{
    using SetTheory.Structs;

    public class Pipeline
    {
        public static Result<List<SiplificationDescription>> Process(string input)
        {
            var tokensResult = input.TryTokenize();
            if (!tokensResult.HasValue)
            {
                var errorIndex = GetErrorIndex(tokensResult.ErrorPosition);
                var token = tokensResult.Remainder.First(1).ToStringValue();
                // use localized user error messages
                return new Result<List<SiplificationDescription>>($"Syntax error for input '{token}'", errorIndex, token);
            }

            var parseResult = tokensResult.Value.TryParse();
            if (!parseResult.HasValue)
            {
                var errorIndex = GetErrorIndex(parseResult.ErrorPosition);
                var token = parseResult.Remainder.ConsumeToken().Value.ToStringValue();
                // use localized user error messages
                return new Result<List<SiplificationDescription>>($"Syntax error for input '{token}'", errorIndex, token);
            }

            var result = Evaluator.Evaluate(parseResult.Value);
            return new Result<List<SiplificationDescription>>(result);
        }

        static int GetErrorIndex(Position position)
        {
            return position.HasValue && position.Line == 1
                    ? position.Column - 1
                    : -1;
        }
    }
}