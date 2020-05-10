using System;
using System.Collections.Generic;

namespace SetTheory
{
    public static class Search
    {
        public static void DFSPostOrder(this Expression current, Action<Expression> action)
            => DFSPostOrderInternal(current, action, new HashSet<Expression>());
        public static void BFSPostOrder(this Expression current, Action<Expression> action)
            => BFSInternal(current, action, new HashSet<Expression>());

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

        static void BFSInternal(
            Expression current,
            Action<Expression> action,
            HashSet<Expression> visited)
        {
            if (visited.Contains(current))
                return;

            visited.Add(current);

            action?.Invoke(current);

            foreach (var child in current.Children)
                BFSInternal(child, action, visited);
        }
    }
}