using DiscreteMath.Core.Language;
using DiscreteMath.Core.Structs;
using System.Collections.Generic;

namespace DiscreteMath.Core.Pipeline
{
    class Simplifier
    {
        readonly PatternMatcher patternMatcher;
        readonly Normalizer normalizer;
        readonly Printer printer;

        public Simplifier(PatternMatcher patternMatcher, Normalizer normalizer, Printer printer)
        {
            this.patternMatcher = patternMatcher;
            this.normalizer = normalizer;
            this.printer = printer;
        }

        public List<SimplificationDescription> Run(Expression expr)
        {
            var used = new HashSet<string>();
            while (true)
            {
                var normalizationResult = normalizer.Normalize(expr);

                if (normalizationResult.HasValue)
                {
                    expr = normalizationResult.Value.ResultingExpression;
                    used.Add(expr.ToString());

                    if (!string.IsNullOrEmpty(normalizationResult.Value.Description))
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