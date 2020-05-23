using DiscreteMath.Core.Language;
using DiscreteMath.Core.Structs;
using DiscreteMath.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscreteMath.Core.Pipeline
{
    class Simplifier
    {
        readonly PatternMatcher patternMatcher;
        readonly Printer printer;

        public Simplifier(PatternMatcher patternMatcher, Printer printer)
        {
            this.patternMatcher = patternMatcher;
            this.printer = printer;
        }

        public List<SimplificationDescription> Run(Expression expr)
        {
            var substitutions = new List<Substitution>();
            var simplifiedNodes = new HashSet<Guid>();

            var expression = expr;

            while (true)
            {
                expression = expression.Normalize();

                var subNodes = expression
                    .AsEnumerable()
                    .Where(x => x.IsOperator())
                    .Where(x => !simplifiedNodes.Contains(x.Id))
                    .ToList();

                if (!subNodes.Any())
                    return substitutions.Select(x => printer.Add(x)).ToList();

                var nodeSimplificationPair = subNodes
                    .Select(x => new { Simplification = SimplifySubExpression(x), InitialNode = x })
                    .FirstOrDefault(x => x.Simplification.HasValue);

                if (nodeSimplificationPair is null)
                    return substitutions.Select(x => printer.Add(x)).ToList();

                var simplificaton = nodeSimplificationPair.Simplification.Value;

                var initialNode = nodeSimplificationPair.InitialNode;
                var resultingNode = simplificaton.ResultedExpression;

                var resultingExpression = expression.SubstituteNode(initialNode.Id, resultingNode);
                var adjustedSubstitutions = AdjustSubstitutions(simplificaton.Substitutions, initialNode.Id, expression);
                expression = resultingExpression;

                substitutions.AddRange(adjustedSubstitutions);

                resultingNode.AsEnumerable()
                    .Select(x => x.Id)
                    .ForEach(x => simplifiedNodes.Add(x));
            }
        }

        MyResult<Simplification> SimplifySubExpression(Expression expr)
        {
            var expression = expr;
            var substitutions = new List<Substitution>();
            var usedExpressions = new HashSet<string>
            {
                expression.ToString()
            };

            while (true)
            {
                expression = expression.Normalize();

                var substitutionResult = patternMatcher.Match(expression, usedExpressions);

                if (!substitutionResult.HasValue)
                    break;

                var substitution = substitutionResult.Value;

                expression = substitution.ResultingExpression.Normalize();

                if (usedExpressions.Contains(expression.ToString()))
                    break;

                substitutions.Add(substitution);
                usedExpressions.Add(expression.ToString());
            }

            if (substitutions.Any())
                return new Simplification { Substitutions = substitutions, ResultedExpression = expression };

            return (Simplification) null;
        }

        List<Substitution> AdjustSubstitutions(List<Substitution> subs, Guid id, Expression expression)
        {
            var substitutions = new List<Substitution>();

            var initialExpression = expression.Clone();
            var currentId = id;

            foreach (var sub in subs)
            {
                var resultingExpression = initialExpression.SubstituteNode(currentId, sub.ResultingExpression);

                var item = new Substitution
                {
                    InitialExpression = initialExpression,
                    InitialPart = sub.InitialPart,
                    ResultingExpression = resultingExpression,
                    ResultingPart = sub.ResultingPart,
                    Description = sub.Description
                };

                substitutions.Add(item);

                initialExpression = resultingExpression;
                currentId = sub.ResultingExpression.Id;
            }

            return substitutions;
        }
    }
}