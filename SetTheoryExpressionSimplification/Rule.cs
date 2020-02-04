using SetTheory.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace SetTheory
{
    public class Rule
    {
        public Rule(Expression pattern, Expression result, string description)
        {
            Pattern = pattern;
            Result = result;
            Description = description;
        }

        Expression Pattern { get; }
        Expression Result { get; }
        string Description { get; }

        public (bool, Expression, Expression, string) Apply(Expression expr)
        {
            var dict = new Dictionary<string, Expression>();
            if (Match(expr, Pattern, ref dict))
            {
                var copy = DeepCopy(Result);
                var expr2 = Substitute(copy, dict);
                return (true, expr, expr2, Description);
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
                copy.DFSPostOrder(actionOnChildren: x => x = x.Select(y => y.Copy()).ToArray());
                return copy;
            }

            static Expression Substitute(Expression result, Dictionary<string, Expression> dict)
            {
                var copy = Sub(result);
                copy.DFSPostOrder(x => x = x.Select(y => Sub(y)).ToArray());
                return copy;

                Expression Sub(Expression expr)
                {
                    if (expr.Value.StartsWith("_"))
                        return dict[expr.Value];
                    return expr;
                }
            }
        }

        internal static Rule FromString(string rule, string description)
        {
            var array = rule.Split('=', 2);
            var matchPattern = array[0];
            var result = array[1];
            var pattern = Parse(matchPattern);
            var substitute = Parse(result);
            return new Rule(pattern, substitute, description);
        }

        static Expression Parse(string pattern)
        {
            // handle errors
            var tokens = pattern.TryTokenizeWithVariables();
            var result = tokens.Value.TryParseRule();
            return result.Value;
        }
    }
}