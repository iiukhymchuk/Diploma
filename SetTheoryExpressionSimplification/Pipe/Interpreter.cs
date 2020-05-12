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
                    used.Add(expr.ToString());
                    printer.Add(normalizationResult.Value);
                }

                var evaluationResult = patternMatcher.Match(expr, used);

                if (evaluationResult.HasValue)
                {
                    expr = evaluationResult.Value.ResultingExpression;
                    used.Add(expr.ToString());
                    printer.Add(evaluationResult.Value);
                }

                if (!normalizationResult.HasValue && !evaluationResult.HasValue)
                    return printer.GetLines();
            }
        }
    }
}