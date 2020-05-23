using DiscreteMath.Core.Language;
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
        readonly RuleApplier ruleApplier;

        public PatternMatcher(List<Rule> rules, RuleApplier ruleApplier)
        {
            this.rules = rules;
            this.ruleApplier = ruleApplier;
        }

        internal MyResult<Substitution> Match(Expression expr, HashSet<string> usedExpressions)
        {
            var copy = expr.Clone();

            var allTreeNodesVariations2 = GetCommutativityPermutations(copy)
                .SelectMany(permutation
                    => permutation
                        .AsEnumerable()
                        .Where(x => x.HasChildren())
                        .Select(x => new { Permutation = permutation, NodeVariation = x }))
                .DistinctBy(x => x.NodeVariation.ToString())
                .ToList();

            foreach (var rule in rules)
                foreach (var item in allTreeNodesVariations2)
                {
                    var resultingPart = ruleApplier.ApplyRuleWithCache(item.NodeVariation, rule);

                    if (resultingPart is null)
                        continue;

                    var resultingExpression = item.Permutation
                        .SubstituteNode(item.NodeVariation.Id, resultingPart)
                        .Normalize();

                    if (usedExpressions.Contains(resultingExpression.ToString()))
                        continue;

                    return new Substitution
                    {
                        InitialExpression = item.Permutation.Copy(),
                        InitialPart = item.NodeVariation.Copy(),
                        ResultingExpression = resultingExpression.Copy(),
                        ResultingPart = resultingPart.Copy(),
                        Description = rule.Description
                    };
                }

            return (Substitution)null;
        }

        static IEnumerable<Expression> GetCommutativityPermutations(Expression expr, Guid? guid = null)
        {
            var guidIsMet = false;

            foreach (var child in expr.AsEnumerable().SkipWhile(x => !ShouldTake(x)))
            {
                if (child.Type == typeof(Intersection) || child.Type == typeof(Union) || child.Type == typeof(SymmetricDifference))
                {
                    foreach (var p in child.Children
                        .GetPermutations(child.Children.Length)
                        .Select(x => x.SplitInTwo(child))
                        .SelectMany(x => x)
                        .ToList())
                    {
                        var copy = expr.Clone();
                        var sub = copy.AsEnumerable().First(x => x.Id == child.Id);
                        sub.Children = p.ToArray();

                        foreach (var tree in GetCommutativityPermutations(copy, sub.Id))
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
    }
}