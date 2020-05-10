using System;
using System.Collections.Generic;

namespace SetTheory
{
    public static class Search
    {
        public static void DFSPostOrder(this Expression current, Action<Expression> action)
            => DFSPostOrderInternal(current, action, new HashSet<Expression>());

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