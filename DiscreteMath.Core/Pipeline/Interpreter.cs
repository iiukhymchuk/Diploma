using DiscreteMath.Core.Language;
using DiscreteMath.Core.Structs;
using System.Collections.Generic;

namespace DiscreteMath.Core.Pipeline
{
    class Interpreter
    {
        private readonly PatternMatcher patternMatcher;
        private readonly Normalizer normalizer;
        private readonly Printer printer;

        public Interpreter(PatternMatcher patternMatcher, Normalizer normalizer)
        {
            this.patternMatcher = patternMatcher;
            this.normalizer = normalizer;
            printer = new Printer();
        }

        public List<SimplificationDescription> Interpretate(Expression expr)
        {
            printer.Add(
                new Substitution
                {
                    ResultingExpression = expr,
                    Description = "Initial value"
                });

            var used = new HashSet<string>();
            while (true)
            {
                var normalizationResult = normalizer.Normalize(expr);

                if (normalizationResult.HasValue)
                {
                    expr = normalizationResult.Value.ResultingExpression;
                    used.Add(expr.Debug);
                    printer.Add(normalizationResult.Value);
                }

                var evaluationResult = patternMatcher.Match(expr, used);

                if (evaluationResult.HasValue)
                {
                    expr = evaluationResult.Value.ResultingExpression;
                    used.Add(expr.Debug);
                    printer.Add(evaluationResult.Value);
                }

                if (!normalizationResult.HasValue && !evaluationResult.HasValue)
                    return printer.GetLines();
            }
        }
    }
}