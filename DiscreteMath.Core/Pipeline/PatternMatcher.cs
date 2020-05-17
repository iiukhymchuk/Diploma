using DiscreteMath.Core.Language;
using DiscreteMath.Core.Language.AST;
using DiscreteMath.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscreteMath.Core.Pipeline
{
    class PatternMatcher
    {
        readonly List<Rule> rules;
        const int MaxSubstitutionsPerRun = 32;
        const double SubstitutionSimplicityConstant = 0.99d;

        public PatternMatcher(List<Rule> rules)
        {
            this.rules = rules;
        }

        internal Result<Substitution> Match(Expression expr, HashSet<string> used)
        {
            var substitutionsLoolup = new HashSet<string>();
            var substitutions = new List<Substitution>();

            foreach (var tree in GetOperationPermutations(expr.Copy(true)))
                foreach (var node in tree.IterateDFSPostOrder())
                    foreach (var rule in rules)
                    {
                        var sub = MatchPattern(tree.Copy(), node, rule);

                        if (sub is null)
                            continue;

                        var stringRepresentation = sub.ResultingExpression.Debug;
                        if (!used.Contains(stringRepresentation) && !substitutionsLoolup.Contains(stringRepresentation))
                        {
                            substitutionsLoolup.Add(stringRepresentation);
                            substitutions.Add(sub);
                        }

                        if (substitutions.Any() && substitutions.Count() >= MaxSubstitutionsPerRun)
                        {
                            var simplest = substitutions.OrderBy(CountSubstitutionSimplicity).First();

                            if (CountSubstitutionSimplicity(simplest) < SubstitutionSimplicityConstant)
                                return new Result<Substitution>(simplest);

                            substitutions.Clear();
                            substitutions.Add(simplest);
                        }
                    }

            if (substitutions.Any())
                return new Result<Substitution>(substitutions.OrderBy(CountSubstitutionSimplicity).First());

            return Result<Substitution>.Empty();
        }

        double CountSubstitutionSimplicity(Substitution substitution)
        {
            return SimplicityCount(substitution.ResultingPart) / SimplicityCount(substitution.InitialPart);

            static double SimplicityCount(Expression x)
            {
                var skip = x.Type == typeof(Parens) ? 1 : 0;
                return x.IterateBFSPostOrder().Skip(skip).Sum(e => 1d);
            }
        }

        static IEnumerable<Expression> GetOperationPermutations(Expression expr, Guid? guid = null)
        {
            var guidIsMet = false;

            foreach (var child in expr.IterateDFSPostOrder().SkipWhile(x => !ShouldTake(x)))
            {
                if (child.Type == typeof(Intersection) || child.Type == typeof(Union))
                {
                    foreach (var p in child.Children
                        .GetPermutations(child.Children.Length)
                        .Select(x => x.SplitInTwo(child))
                        .SelectMany(x => x))
                    {
                        var copy = expr.Copy(true);
                        var sub = copy.IterateDFSPostOrder().First(x => x.Id == child.Id);
                        sub.Children = p.ToArray();

                        foreach (var tree in GetOperationPermutations(copy, sub.Id))
                        {
                            yield return tree;
                        }
                    }

                    yield break;
                }
            }

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

            if (!CheckMatch(expression, rule.PatternIn, matchSubstitutions))
                return null;

            return Substitute(tree, expression, rule, matchSubstitutions);
        }

        static bool CheckMatch(
            Expression toBeMatched,
            Expression pattern,
            Dictionary<string, Expression> matchSubstitutions)
        {
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
                    if (sub is Operation)
                        sub = new Parens(sub);

                    pattern = sub;
                }
                pattern.Children = pattern.Children.Select(x => GetSubstitutedVariable(x)).ToArray();
                return pattern;
            }

            static Expression GetSubstitutedExpression(Expression tree, Expression initial, Expression resulting)
            {
                var result = tree.Copy();
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
    }
}