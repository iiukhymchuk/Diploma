using System.Diagnostics;
using System.Linq;

namespace DiscreteMath.Core.Language.AST
{
    [DebuggerDisplay("{Debug}")]
    public class SymmetricDifference : Operation
    {
        public SymmetricDifference(string value, params Expression[] children) : base(value, children) { }

        public override Expression Copy(bool copyId = false)
            => new SymmetricDifference(Value, Children.Select(x => x.Copy(copyId)).ToArray());
    }
}