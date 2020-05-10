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
                expr.DFSPostOrder(x => ApplyRule(expr, x, rule, substitutions));
                if (substitutions.Any())
                    return new Result<Substitution>(substitutions.First());
            }
            return Result<Substitution>.Empty();
        }

        static void ApplyRule(Expression expr, Expression current, Rule rule, List<Substitution> substitutions)
        {
            var substitutionsTable = new Dictionary<string, Expression>();
            var partialMatches = new Dictionary<string, IEnumerable<Expression>>();

            var doMatch = CheckMatch(current.Copy(), rule.PatternIn, substitutionsTable, partialMatches);
            if (!doMatch)
                return;

            var (substitutedExpr, initial, resulting) = Substitute(expr, current, rule, substitutionsTable, partialMatches);

            substitutions.Add(
                new Substitution
                {
                    Expression = substitutedExpr,
                    InitialPart = initial,
                    ResultingPart = resulting,
                    Description = rule.Description
                });
        }

        static bool CheckMatch(
            Expression current,
            Expression pattern,
            Dictionary<string, Expression> substitutions,
            Dictionary<string, IEnumerable<Expression>> partialMatches)
        {
            if (IsVariable(pattern))
            {
                var exists = substitutions.TryGetValue(pattern.Value, out var substitution);
                if (exists)
                    return Utils.ExprEquals(current, substitution);

                substitutions[pattern.Value] = current.Copy();
                return true;
            }

            if (!IsSameExpression(current, pattern))
                return false;

            if (pattern.Children.Length == 0)
                return true;

            var allPermutations = current.Children.GetPermutations(pattern.Children.Length).ToList();

            var partialMatch = allPermutations
                .Select(x => new { Permutation = x, NewSubs = new Dictionary<string, Expression>(substitutions) })
                .Where(x => Utils.CollectionsEquals(
                    x.Permutation.ToArray(),
                    pattern.Children,
                    (a, b) => CheckMatch(a, b, x.NewSubs, partialMatches)))
                .FirstOrDefault();

            if (partialMatch is null)
                return false;
            else
            {
                substitutions.Clear();
                foreach (var item in partialMatch.NewSubs)
                {
                    substitutions.Add(item.Key, item.Value);
                }
            }

            if (current.Children.Length != pattern.Children.Length)
                partialMatches[pattern.Value] = partialMatch.Permutation;

            return true;
        }

        static (Expression, Expression, Expression) Substitute(
            Expression expr,
            Expression initial,
            Rule rule,
            Dictionary<string, Expression> substitutionsDictionary,
            Dictionary<string, IEnumerable<Expression>> partialMatches)
        {
            var initialValue = initial.Copy();

            var substitutedPattern = GetSubstitutedVariable(rule.PatternOut);
            var resulting = GetResultingValue(initialValue, rule.PatternIn, substitutedPattern);
            var substitutedExpr = GetSubstitutedExpression(expr, initialValue, resulting);
            return (substitutedExpr, initialValue, resulting);

            Expression GetSubstitutedVariable(Expression patternOut)
            {
                var pattern = patternOut.Copy();
                if (IsVariable(pattern))
                    pattern = substitutionsDictionary[pattern.Value];
                pattern.Children = pattern.Children.Select(x => GetSubstitutedVariable(x)).ToArray();
                return pattern;
            }

            Expression GetResultingValue(Expression initialValue, Expression patternIn, Expression substitutedPattern)
            {
                var expr = initialValue.Copy();
                if (partialMatches.TryGetValue(patternIn.Value, out var matched))
                {
                    expr.Children = expr.Children
                        .Where(x => !matched.Any(match => Utils.ExprEquals(x, match)))
                        .Concat(new[] { substitutedPattern })
                        .ToArray();
                    return expr;
                }

                return substitutedPattern.Copy();
            }

            static Expression GetSubstitutedExpression(Expression expr, Expression initial, Expression resulting)
            {
                var result = expr.Copy();
                resulting = resulting.Copy();
                result.DFSPostOrder(
                    x => x.Children = x.Children.Select(y => Utils.ExprEquals(y, initial) ? resulting : y).ToArray());
                return result;
            }
        }

        static bool IsVariable(Expression pattern) => pattern.Value.StartsWith("_");
        static bool IsSameExpression(Expression expr1, Expression expr2)
            => expr1.Type == expr2.Type && expr1.Value == expr2.Value;
    }
}