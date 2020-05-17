using DiscreteMath.Core.Structs;
using System.Collections.Generic;

namespace DiscreteMath.Core.Pipeline
{
    class Printer
    {
        readonly List<SimplificationDescription> lines = new List<SimplificationDescription>();

        internal void Add(Substitution value)
        {
            var appliedRulePresent = !(value.InitialPart is null || value.ResultingPart is null);
            lines.Add(new SimplificationDescription
            {
                SimplifiedExpression = value.ResultingExpression?.ToString() ?? string.Empty,
                AppliedRule = appliedRulePresent ? $"{value.InitialPart} => {value.ResultingPart}" : string.Empty,
                RuleDescription = value.Description
            });
        }

        internal List<SimplificationDescription> GetLines()
        {
            return lines;
        }
    }
}