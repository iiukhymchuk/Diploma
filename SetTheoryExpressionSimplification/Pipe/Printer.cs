using System.Collections.Generic;

namespace SetTheory
{
    class Printer
    {
        readonly List<SimplificationDescription> lines = new List<SimplificationDescription>();

        internal void Add(Substitution value)
        {
            var appliedRulePresent = !(value.Initial is null || value.Resulting is null);
            lines.Add(new SimplificationDescription
            {
                SimplifiedExpression = value.Expression?.ToString() ?? string.Empty,
                AppliedRule = appliedRulePresent ? $"{value.Initial} => {value.Resulting}" : string.Empty,
                RuleDescription = value.Description
            });
        }

        internal List<SimplificationDescription> GetLines()
        {
            return lines;
        }
    }
}