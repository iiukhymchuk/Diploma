using System.Collections.Generic;
using System.Linq;

namespace SetTheory
{
    class Interpreter
    {
        private readonly PatternMatcher patternMatcher;
        private readonly Normalizer normalizer;

        public Interpreter(PatternMatcher patternMatcher, Normalizer normalizer)
        {
            this.patternMatcher = patternMatcher;
            this.normalizer = normalizer;
        }

        public List<SimplificationDescription> Interpretate(Tree expr)
        {
            var lines = new List<SimplificationDescription>();

            while (true)
            {
                // normalize expression
                expr = normalizer.Normalize(expr);

                // match expression
                var result = patternMatcher.Match(expr);

                if (!result.HasValue)
                    return lines;

                // print
                expr = ApplyPattern(expr, result.Value.Initial, result.Value.Resulting);
                lines.Add(new SimplificationDescription
                {
                    SimplifiedExpression = expr.ToString(),
                    AppliedRule = $"{result.Value.Initial} => {result.Value.Resulting}",
                    RuleDescription = result.Value.Description
                });
            }
        }

        static Tree ApplyPattern(Tree expr, Expression initial, Expression resulting)
        {
            var tree = (Tree) expr.Copy();
            tree.DFSPostOrder(
                x => x.Children = x.Children.Select(y => Expression.ExprEquals(y, initial) ? resulting : y).ToArray());
            return tree;
        }
    }
}