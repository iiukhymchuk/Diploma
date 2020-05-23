using DiscreteMath.Core.Language;
using System.Collections.Generic;

namespace DiscreteMath.Core.Structs
{
    internal class Simplification
    {
        public List<Substitution> Substitutions { get; set; }
        public Expression ResultedExpression { get; set; }
    }
}