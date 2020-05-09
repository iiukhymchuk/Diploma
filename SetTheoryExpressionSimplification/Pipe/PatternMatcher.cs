using System.Collections.Generic;
using System.Linq;

namespace SetTheory
{
    class PatternMatcher
    {
        internal Result<Substitution> Match(Expression expr)
        {
            return expr.DFSPostOrder(x => ApplyRules(x, new Rules()));
        }

        static Result<Substitution> ApplyRules(Expression expr, Rules rules)
        {
            foreach (var rule in rules)
            {
                var result = ApplyRule(expr, rule);
                if (result.HasValue)
                    return result;
            }
            return Result<Substitution>.Empty();
        }

        static Result<Substitution> ApplyRule(Expression expr, Rule rule)
        {
            var dict = new Dictionary<string, Expression>();
            if (Match(expr, rule.PatternIn, ref dict))
            {
                var copy = rule.PatternOut.Copy();
                var expr2 = Substitute(copy, dict);
                var value = new Substitution
                {
                    Matched = expr,
                    Resulting = expr2,
                    Description = rule.Description
                };
                return new Result<Substitution>(value);
            }

            return Result<Substitution>.Empty();

            static bool Match(
                Expression expr,
                Expression pattern,
                ref Dictionary<string, Expression> vars)
            {
                if (pattern.Value.StartsWith("_")) // is variable
                    return MatchVariable(expr, pattern, ref vars);

                if (expr.Type != pattern.Type) return false;
                if (expr.Value != pattern.Value) return false;
                if (expr.Children.Length != pattern.Children.Length) return false;

                foreach (var two in expr.Children.Zip(pattern.Children, (x, y) => (x, y)))
                {
                    if (!Match(two.x, two.y, ref vars))
                        return false;
                }
                return true;
            }

            static bool MatchVariable(
                Expression expr,
                Expression pattern,
                ref Dictionary<string, Expression> vars)
            {
                var exists = vars.TryGetValue(pattern.Value, out var existingExpr);
                if (exists)
                    return Expression.ExprEquals(expr, existingExpr);

                vars[pattern.Value] = expr;
                return true;
            }

            static Expression Substitute(Expression result, Dictionary<string, Expression> dict)
            {
                var copy = Sub(result);
                copy.DFSPostOrder(x => x.Children = x.Children.Select(y => Sub(y)).ToArray());
                return copy;

                Expression Sub(Expression expr)
                {
                    if (expr.Value.StartsWith("_"))
                        return dict[expr.Value];
                    return expr;
                }
            }
        }
    }
}