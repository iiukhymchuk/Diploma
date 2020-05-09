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

            var changed = true;
            while (changed)
            {
                changed = false;

                // normalize expression
                var normalizedExpr = normalizer.Normalize(expr);

                // match expression
                var result = patternMatcher.Match(normalizedExpr);

                // print
                if (result.HasValue)
                {
                    changed = true;
                    var copy = ApplyPattern(normalizedExpr, result.Value.Initial, result.Value.Resulting);
                    expr = copy;
                    lines.Add(new SimplificationDescription
                    {
                        SimplifiedExpression = copy.ToString(),
                        AppliedRule = $"{result.Value.Initial} => {result.Value.Resulting}",
                        RuleDescription = result.Value.Description
                    });
                }
            }

            return lines;
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