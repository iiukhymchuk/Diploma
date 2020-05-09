using System.Collections.Generic;
using System.Linq;

namespace SetTheory
{
    public class Evaluator
    {
        public static List<SimplificationDescription> Evaluate(Expression expr)
        {
            var lines = new List<SimplificationDescription>();

            var changed = true;
            while (changed)
            {
                changed = false;

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
            }

            return lines;
        }

        private static Expression Substitute(Expression expr, Expression initial, Expression expression)
        {
            var copy = Sub(expr, initial, expression);
            copy.DFSPostOrder(x => copy.Children = x.Select(y => Sub(y, initial, expression)).ToArray());
            return copy;

            static Expression Sub(Expression expr, Expression initial, Expression expression)
            {
                if (ExpressionUtils.ExprEquals(expr, initial))
                    return expression;
                return expr;
            }
        }

        static (bool, Expression, Expression, string) ApplyRules(Expression expr, Rules rules)
        {
            foreach (var rule in rules)
            {
                var (success, initial, resulting, description) = rule.Apply(expr);
                if (success) return (success, initial, resulting, description);
            }
            return (false, null, null, null);
        }
    }
}