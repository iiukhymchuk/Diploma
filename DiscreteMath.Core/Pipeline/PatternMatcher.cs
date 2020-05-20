using DiscreteMath.Core.Language;
using DiscreteMath.Core.Language.AST;
using DiscreteMath.Core.Structs;
using DiscreteMath.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscreteMath.Core.Pipeline
{
    class PatternMatcher
    {
        readonly List<Rule> rules;
        const int MaxSubstitutionsPerRun = 256;
        const double SubstitutionSimplicityConstant = 0.45d;

        public PatternMatcher(List<Rule> rules)
        {
            this.rules = rules;
        }

        static Cache<Guid, string, SimplificationDescription> cache = new Cache<Guid, string, SimplificationDescription>();

        internal MyResult<Substitution> Match(Expression expr, HashSet<string> used)
        {
            var substitutionsLookup = new HashSet<string>();
            var substitutions = new List<Substitution>();

            foreach (var tree in GetOperationPermutations(expr.Copy(true)))
                foreach (var node in tree.AsEnumerableDFSPostOrder())
                    foreach (var rule in rules)
                    {
                        var sub = MatchPattern(tree.Copy(), node, rule);

                        if (sub is null)
                            continue;

                        if (rule.Precidence < 0.11d)
                            return new MyResult<Substitution>(sub);

                        var stringRepresentation = sub.ResultingExpression.Debug;
                        if (!used.Contains(stringRepresentation) && !substitutionsLookup.Contains(stringRepresentation))
                        {
                            substitutionsLookup.Add(stringRepresentation);
                            substitutions.Add(sub);
                        }

                        if (substitutions.Any() && substitutions.Count() >= MaxSubstitutionsPerRun)
                        {
                            var ordered = substitutions.OrderBy(x => x.Precidence).ThenBy(CountSubstitutionSimplicity);
                            var simplest = ordered.First();

                            if (simplest.Precidence < SubstitutionSimplicityConstant)
                                return new MyResult<Substitution>(simplest);

                            substitutions.Clear();
                            substitutions.Add(simplest);
                        }
                    }

            if (substitutions.Any())
            {
                var simplest = substitutions.OrderBy(x => x.Precidence).ThenBy(CountSubstitutionSimplicity).First();
                return new MyResult<Substitution>(simplest);
            }

            return MyResult<Substitution>.Empty();
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

            foreach (var child in expr.AsEnumerableDFSPostOrder().SkipWhile(x => !ShouldTake(x)))
            {
                if (child.Type == typeof(Intersection) || child.Type == typeof(Union))
                {
                    foreach (var p in child.Children
                        .GetPermutations(child.Children.Length)
                        .Select(x => x.SplitInTwo(child))
                        .SelectMany(x => x))
                    {
                        var copy = expr.Copy(true);
                        var sub = copy.AsEnumerableDFSPostOrder().First(x => x.Id == child.Id);
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
            if (pattern.IsVariable())
            {
                var exists = matchSubstitutions.TryGetValue(pattern.Value, out var substitution);
                if (exists)
                    return TreeUtils.ExprEquals(toBeMatched, substitution);

                matchSubstitutions[pattern.ToString()] = toBeMatched.Copy();
                return true;
            }

            if (!toBeMatched.IsSameExpression(pattern))
                return false;

            if (!pattern.HasChildren())
                return true;

            if (toBeMatched.Children.Length < pattern.Children.Length)
                return false;

            return TreeUtils.CollectionsEquals(toBeMatched.Children, pattern.Children, (a, b) => CheckMatch(a, b, matchSubstitutions));
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
                Description = rule.Description,
                Precidence = rule.Precidence
            };

            Expression GetSubstitutedVariable(Expression pattern)
            {
                if (pattern.IsVariable())
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
                    x => x.Children = x.Children.Select(y => TreeUtils.ExprEquals(y, initial) ? resulting : y).ToArray());
                return result;
            }
        }
    }
}