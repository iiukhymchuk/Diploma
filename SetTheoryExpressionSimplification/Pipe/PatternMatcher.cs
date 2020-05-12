using System;
using System.Collections.Generic;
using System.Linq;

namespace SetTheory
{
    class PatternMatcher
    {
        readonly List<Rule> rules;
        const int MaxResultsPerTurn = 10;

        public PatternMatcher(List<Rule> rules)
        {
            this.rules = rules;
        }

        internal Result<Substitution> Match(Expression expr, HashSet<string> used)
        {
            var substitutions = new List<Substitution>();

            foreach (var tree in GetOperationPermutations(expr.Copy()))
                foreach (var node in tree.IterateDFSPostOrder())
                    foreach (var rule in rules)
                    {
                        var sub = MatchPattern(tree.Copy(), node, rule);

                        if (sub != null && !IsAlreadyPresent(sub.ResultingExpression.ToString()))
                            substitutions.Add(sub);

                        if (substitutions.Count() >= MaxResultsPerTurn)
                            goto resultSelection;
                    }

        resultSelection:

            if (substitutions.Any())
                return new Result<Substitution>(substitutions
                    .OrderBy(x => SimplicityCount(x.ResultingExpression))
                    .First());

            return Result<Substitution>.Empty();

            bool IsAlreadyPresent(string expressionString) =>
                used.Contains(expressionString)
                    || substitutions.Any(s => s.ResultingExpression.ToString() == expressionString);
        }

        static IEnumerable<Expression> GetOperationPermutations(Expression expr, Guid? guid = null)
        {
            bool alreadyReturned = false;
            var guidIsMet = false;

            var nodes = expr.IterateDFSPostOrder().SkipWhile(x => !ShouldTake(x)).ToList();
            foreach (var child in nodes)
            {
                if (child.Type == typeof(Operation))
                {
                    foreach (var p in child.Children
                        .GetPermutations(child.Children.Length)
                        .Select(x => x.SplitInTwo(child))
                        .SelectMany(x => x))
                    {
                        var copy = expr.Copy(true);
                        var sub = copy.IterateDFSPostOrder().First(x => x.Id == child.Id);
                        sub.Children = p.ToArray();

                        alreadyReturned = true;
                        foreach (var tree in GetOperationPermutations(copy, sub.Id))
                        {
                            yield return tree;
                        }
                    }
                }
            }

            if (!alreadyReturned)
                yield return expr;

            bool ShouldTake(Expression x)
            {
                var shouldTake = guid is null || guidIsMet;
                if (x.Id == guid)
                    guidIsMet = true;

                return shouldTake;
            }
        }

        static Substitution MatchPattern(
            Expression tree,
            Expression expression,
            Rule rule)
        {
            var matchSubstitutions = new Dictionary<string, Expression>();

            var exprCopy = expression.Copy();

            if (!CheckMatch(exprCopy, rule.PatternIn, matchSubstitutions))
                return null;

            return Substitute(tree, exprCopy, rule, matchSubstitutions);
        }

        static bool CheckMatch(
            Expression toBeMatched,
            Expression pattern,
            Dictionary<string, Expression> matchSubstitutions = null)
        {
            matchSubstitutions ??= new Dictionary<string, Expression>();

            if (IsVariable(pattern))
            {
                var exists = matchSubstitutions.TryGetValue(pattern.Value, out var substitution);
                if (exists)
                    return Utils.ExprEquals(toBeMatched, substitution);

                matchSubstitutions[pattern.ToString()] = toBeMatched.Copy();
                return true;
            }

            if (!IsSameExpression(toBeMatched, pattern))
                return false;

            if (!HasChildren(pattern))
                return true;

            if (toBeMatched.Children.Length < pattern.Children.Length)
                return false;

            return Utils.CollectionsEquals(toBeMatched.Children, pattern.Children, (a, b) => CheckMatch(a, b, matchSubstitutions));
        }

        static Substitution Substitute(
            Expression tree,
            Expression partialTree,
            Rule rule,
            Dictionary<string, Expression> substitutionsDictionary)
        {
            var substitutedPattern = GetSubstitutedVariable(rule.PatternOut.Copy());
            var substitutedExpr = GetSubstitutedExpression(tree, partialTree, substitutedPattern);

            return new Substitution
            {
                InitialExpression = tree.Copy(),
                ResultingExpression = substitutedExpr,
                InitialPart = partialTree,
                ResultingPart = substitutedPattern,
                Description = rule.Description
            };

            Expression GetSubstitutedVariable(Expression pattern)
            {
                if (IsVariable(pattern))
                {
                    var sub = substitutionsDictionary[pattern.ToString()];
                    if (sub.Type == typeof(Operation))
                        sub = new Parens(sub);

                    pattern = sub;
                }
                pattern.Children = pattern.Children.Select(x => GetSubstitutedVariable(x)).ToArray();
                return pattern;
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
        static bool HasChildren(Expression pattern) => pattern.Children.Length != 0;
        static bool IsSameExpression(Expression expr1, Expression expr2)
            => expr1.Type == expr2.Type && expr1.Value == expr2.Value;
        static int SimplicityCount(Expression x) => x.IterateDFSPostOrder().Sum(e => 1);
    }
}