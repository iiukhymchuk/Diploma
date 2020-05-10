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
                var substitutions = new List<Substitution>();
                expr.DFSPostOrder(x => ApplyRule(x, rule, substitutions));
                if (substitutions.Any())
                {
                    var substitution = substitutions.First();
                    substitution.Expression = Substitute(expr, substitution.Initial, substitution.Resulting);
                    return new Result<Substitution>(substitution);
                }
            }
            return Result<Substitution>.Empty();
        }

        static void ApplyRule(Expression current, Rule rule, List<Substitution> substitutions)
        {
            var substitutionsDictionary = new Dictionary<string, Expression>();

            var doMatch = Match(current, rule.PatternIn, substitutionsDictionary);
            if (!doMatch)
                return;

            var patternOutCopy = rule.PatternOut.Copy();
            var resulting = Substitute(patternOutCopy, substitutionsDictionary);

            substitutions.Add(
                new Substitution
                {
                    Initial = current,
                    Resulting = resulting,
                    Description = rule.Description
                });
        }

        static Expression Substitute(Expression expr, Expression initial, Expression resulting)
        {
            var tree = expr.Copy();
            tree.DFSPostOrder(
                x => x.Children = x.Children.Select(y => Expression.ExprEquals(y, initial) ? resulting : y).ToArray());
            return tree;
        }

        static bool Match2(
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

            if (IsOperation(expr) && IsOperation(pattern))
            {
                //PartialMatch();
            }

            if (expr.Children.Length != pattern.Children.Length) return false;

            foreach (var two in expr.Children.Zip(pattern.Children, (x, y) => (x, y)))
            {
                if (!Match(two.x, two.y, substitutions))
                    return false;
            }
            return true;
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

            if (IsOperation(expr) && IsOperation(pattern))
            {
                //PartialMatch();
            }

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
        static bool IsOperation(Expression expr) => expr.Type == typeof(Operation);
    }
}