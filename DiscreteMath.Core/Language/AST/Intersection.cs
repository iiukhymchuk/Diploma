using System;
using System.Diagnostics;
using System.Linq;

namespace DiscreteMath.Core.Language.AST
{
    [DebuggerDisplay("{Debug}")]
    public class Intersection : Operation
    {
        public Intersection(string value, params Expression[] children) : base(value, children) { }

        public override Expression Copy(bool copyId = false)
            => copyId
                ? new Intersection(Value, Children.Select(x => x.Copy(copyId)).ToArray()) { Id = Id ?? Guid.NewGuid() }
                : new Intersection(Value, Children.Select(x => x.Copy(copyId)).ToArray());
    }
}
