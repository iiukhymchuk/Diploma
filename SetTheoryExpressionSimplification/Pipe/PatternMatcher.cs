using System.Collections.Generic;
using System.Linq;

namespace SetTheory
{
    class PatternMatcher
    {
        readonly List<Rule> rules;

        public PatternMatcher(List<Rule> rules)
        {
            this.rules = rules;
        }

        internal Result<Substitution> Match(Expression expr)
        {
            foreach (var rule in rules)
            {
                var result = expr.DFSPostOrder(x => ApplyRule(x, rule));
                if (result.HasValue)
                {
                    result.Value.Expression = ChangeTree(expr, result.Value.Initial, result.Value.Resulting);
                    return result;
                }
            }
            return Result<Substitution>.Empty();
        }

        static Result<Substitution> ApplyRule(Expression current, Rule rule)
        {
            var substitutions = new Dictionary<string, Expression>();

            var doMatch = Match(current, rule.PatternIn, substitutions);
            if (!doMatch)
                return Result<Substitution>.Empty();

            var patternOutCopy = rule.PatternOut.Copy();
            var resulting = Substitute(patternOutCopy, substitutions);

            return new Result<Substitution>(
                new Substitution
                {
                    Initial = current,
                    Resulting = resulting,
                    Description = rule.Description
                });
        }

        static Expression ChangeTree(Expression expr, Expression initial, Expression resulting)
        {
            var tree = expr.Copy();
            tree.DFSPostOrder(
                x => x.Children = x.Children.Select(y => Expression.ExprEquals(y, initial) ? resulting : y).ToArray());
            return tree;
        }

        static bool Match(
            Expression expr,
            Expression pattern,
            Dictionary<string, Expression> substitutions)
        {
            if (IsVariable(pattern))
            {
                var exists = substitutions.TryGetValue(pattern.Value, out var substitution);
                if (exists)
                    return Expression.ExprEquals(expr, substitution);

                substitutions[pattern.Value] = expr.Copy();
                return true;
            }

            if (expr.Type != pattern.Type) return false;
            if (expr.Value != pattern.Value) return false;
            if (expr.Children.Length != pattern.Children.Length) return false;

            foreach (var two in expr.Children.Zip(pattern.Children, (x, y) => (x, y)))
            {
                if (!Match(two.x, two.y, substitutions))
                    return false;
            }
            return true;
        }

        static Expression Substitute(Expression patternOutCopy, Dictionary<string, Expression> substitutions)
        {
            if (IsVariable(patternOutCopy))
                patternOutCopy = substitutions[patternOutCopy.Value];
            patternOutCopy.Children = patternOutCopy.Children.Select(x => Substitute(x, substitutions)).ToArray();
            return patternOutCopy;
        }

        static bool IsVariable(Expression pattern) => pattern.Value.StartsWith("_");
    }
}