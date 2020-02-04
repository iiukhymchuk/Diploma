using SetTheory.Expressions;
using SetTheory.Structs;
using System.Collections.Generic;
using System.Linq;

namespace SetTheory
{
    public class Evaluator
    {
        public static List<SiplificationDescription> Evaluate(Expression expr)
        {
            var lines = new List<SiplificationDescription>();

            var changed = true;
            while (changed)
            {
                changed = false;

                var (success, initial, substitution, resulting, description) = expr.DFSPostOrder2(x => ApplyRules(x, Rules));
                if (success)
                {
                    changed = true;
                    var copy = Substitute(expr, initial, resulting);
                    expr = copy;
                    lines.Add(new SiplificationDescription
                    {
                        SimplifiedExpression = copy.ToString(),
                        AppliedRule = $"{substitution.ToString()} => {resulting.ToString()}",
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

        static (bool, Expression, Expression, string) ApplyRules(Expression expr, List<Rule> rules)
        {
            foreach (var rule in rules)
            {
                var (success, initial, resulting, description) = rule.Apply(expr);
                if (success) return (success, initial, resulting, description);
            }
            return (false, null, null, null);
        }

        public static List<Rule> Rules { get; } =
            new List<Rule>
            {
                Rule.FromString("_A * _A = _A", "Indempodent rule"),
                Rule.FromString("_A + _A = _A", "Indempodent rule"),
                Rule.FromString("_A + U = U", "Domination rule"),
                Rule.FromString("U + _A = U", "Domination rule"),
                Rule.FromString("_A * O = O", "Domination rule"),
                Rule.FromString("O * _A = O", "Domination rule"),
                Rule.FromString("_A + O = _A", "Identity rule"),
                Rule.FromString("O + _A = _A", "Identity rule"),
                Rule.FromString("_A * U = _A", "Identity rule"),
                Rule.FromString("U * _A = _A", "Identity rule"),
                Rule.FromString("_A + _A' = U", "Complement rule"),
                Rule.FromString("_A * _A' = O", "Complement rule"),
            };
    }
}