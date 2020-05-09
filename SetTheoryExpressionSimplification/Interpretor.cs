using System.Collections.Generic;
using System.Linq;

namespace SetTheory
{
    public class Interpretor
    {
        public static List<SimplificationDescription> Evaluate(Expression expr)
        {
            var lines = new List<SimplificationDescription>();

            var changed = true;
            while (changed)
            {
                changed = false;

                // normalize expression

                // evaluate expression
                var (success, initial, substitution, resulting, description) = expr.DFSPostOrder2(x => ApplyRules(x, new Rules()));
                if (success)
                {
                    changed = true;
                    var copy = Substitute(expr, initial, resulting);
                    expr = copy;
                    lines.Add(new SimplificationDescription
                    {
                        SimplifiedExpression = copy.ToString(),
                        AppliedRule = $"{substitution} => {resulting}",
                        RuleDescription = description
                    });
                }

                // print
            }

            return lines;
        }

        static (bool, Expression, Expression, string) ApplyRules(Expression expr, Rules rules)
        {
            foreach (var rule in rules)
            {
                var (success, initial, resulting, description) = ApplyRule(expr, rule);
                if (success)
                    return (success, initial, resulting, description);
            }
            return (false, null, null, null);
        }

        static (bool, Expression, Expression, string) ApplyRule(Expression expr, Rule rule)
        {
            var dict = new Dictionary<string, Expression>();
            if (Match(expr, rule.PatternIn, ref dict))
            {
                var copy = DeepCopy(rule.PatternOut);
                var expr2 = Substitute(copy, dict);
                return (true, expr, expr2, rule.Description);
            }

            return (false, null, null, null);

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
                    return ExpressionUtils.ExprEquals(expr, existingExpr);

                vars[pattern.Value] = expr;
                return true;
            }

            static Expression DeepCopy(Expression result)
            {
                var copy = result.Copy();
                copy.DFSPostOrder<Expression[]>(x => x = x.Select(y => y.Copy()).ToArray());
                return copy;
            }

            static Expression Substitute(Expression result, Dictionary<string, Expression> dict)
            {
                var copy = Sub(result);
                copy.DFSPostOrder<Expression[]>(x => x = x.Select(y => Sub(y)).ToArray());
                return copy;

                Expression Sub(Expression expr)
                {
                    if (expr.Value.StartsWith("_"))
                        return dict[expr.Value];
                    return expr;
                }
            }
        }

        static Expression Substitute(Expression expr, Expression initial, Expression expression)
        {
            var copy = Sub(expr, initial, expression);
            copy.DFSPostOrder<Expression[]>(x => copy.Children = x.Select(y => Sub(y, initial, expression)).ToArray());
            return copy;

            static Expression Sub(Expression expr, Expression initial, Expression expression)
            {
                if (ExpressionUtils.ExprEquals(expr, initial))
                    return expression;
                return expr;
            }
        }
    }
}