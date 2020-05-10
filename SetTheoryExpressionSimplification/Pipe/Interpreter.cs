using System.Collections.Generic;

namespace SetTheory
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
            this.printer = new Printer();
        }

        public List<SimplificationDescription> Interpretate(Expression expr)
        {
            printer.Add(
                new Substitution
                {
                    Expression = expr,
                    Description = "Initial value"
                });
            while (true)
            {
                var normalizationResult = normalizer.Normalize(expr);

                if (normalizationResult.HasValue)
                {
                    expr = normalizationResult.Value.Expression;
                    printer.Add(normalizationResult.Value);
                }

                var evaluationResult = patternMatcher.Match(expr);

                if (evaluationResult.HasValue)
                {
                    expr = evaluationResult.Value.Expression;
                    printer.Add(evaluationResult.Value);
                }

                if (!normalizationResult.HasValue && !evaluationResult.HasValue)
                    return printer.GetLines();
            }
        }
    }
}