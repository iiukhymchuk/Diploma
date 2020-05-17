using System.Diagnostics;
using System.Linq;

namespace DiscreteMath.Core.Language.AST
{
    [DebuggerDisplay("{Debug}")]
    public class Difference : Operation
    {
        public Difference(string value, params Expression[] children) : base(value, children) { }

        public override Expression Copy(bool copyId = false)
            => new Difference(Value, Children.Select(x => x.Copy(copyId)).ToArray());
    }
}