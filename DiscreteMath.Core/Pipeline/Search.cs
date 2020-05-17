using DiscreteMath.Core.Language;
using System;
using System.Collections.Generic;

namespace DiscreteMath.Core.Pipeline
{
    public static class Search
    {
        public static void DFSPostOrder(this Expression current, Action<Expression> action)
            => DFSPostOrderInternal(current, action, new HashSet<Expression>());
        public static IEnumerable<Expression> IterateDFSPostOrder(this Expression expr)
        {
            foreach (var child in expr.Children)
                foreach (var subChild in child.IterateDFSPostOrder())
                    yield return subChild;
            yield return expr;
        }

        public static IEnumerable<Expression> IterateBFSPostOrder(this Expression expr)
        {
            yield return expr;
            foreach (var child in expr.Children)
                foreach (var subChild in child.IterateBFSPostOrder())
                    yield return subChild;
        }

        static void DFSPostOrderInternal(
            Expression current,
            Action<Expression> action,
            HashSet<Expression> visited)
        {
            if (visited.Contains(current))
                return;

            visited.Add(current);

            foreach (var child in current.Children)
                DFSPostOrderInternal(child, action, visited);

            action?.Invoke(current);
        }
    }
}