using DiscreteMath.Core.Language;
using DiscreteMath.Core.Utils;
using System;
using System.Collections.Generic;

namespace DiscreteMath.Core.Pipeline
{
    class RuleApplier
    {
        static readonly Cache<Guid, string, Expression> cache = new Cache<Guid, string, Expression>();

        internal Expression ApplyRuleWithCache(Expression expression, Rule rule)
            => cache.GetCachedOrExecute(rule.Id, expression.ToString(), () => ApplyRule(expression, rule));

        Expression ApplyRule(Expression expression, Rule rule)
        {
            var variables = new Dictionary<string, Expression>();

            if (CheckMatch(expression, rule.PatternIn, variables))
                return Substitute(rule.PatternOut, variables);

            return null;
        }

        static bool CheckMatch(Expression toBeMatched, Expression pattern, Dictionary<string, Expression> variables)
        {
            if (pattern.IsVariable())
            {
                var exists = variables.TryGetValue(pattern.Value, out var substitution);
                if (exists)
                    return toBeMatched.StrictEquals(substitution);

                variables[pattern.ToString()] = toBeMatched.Copy();
                return true;
            }

            if (!toBeMatched.IsSameExpression(pattern))
                return false;

            if (!pattern.HasChildren())
                return true;

            if (toBeMatched.Children.Length < pattern.Children.Length)
                return false;

            return toBeMatched.Children.PairsComply(pattern.Children, (a, b) => CheckMatch(a, b, variables));
        }

        static Expression Substitute(Expression patternOut, Dictionary<string, Expression> variables)
            => patternOut.AsTree().ChangeTree(x => x.IsVariable(), x => variables[x.ToString()].Copy()).AsExpression();
    }
}