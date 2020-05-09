using System;
using System.Collections.Generic;

namespace SetTheory
{
    public static class Search
    {
        public static void DFSPostOrder(
            this Expression current,
            Action<Expression> action,
            HashSet<Expression> visited = null)
        {
            visited ??= new HashSet<Expression>();
            if (visited.Contains(current))
                return;

            visited.Add(current);

            foreach (var child in current.Children)
            {
                child.DFSPostOrder(action, visited);
            }

            action?.Invoke(current);
        }

        public static Result<Substitution> DFSPostOrder(
            this Expression current,
            Func<Expression, Result<Substitution>> func,
            HashSet<Expression> visited = null)
        {
            visited ??= new HashSet<Expression>();
            if (visited.Contains(current))
                return Result<Substitution>.Empty();

            visited.Add(current);

            foreach (var child in current.Children)
            {
                var childResult = child.DFSPostOrder(func, visited);
                if (childResult.HasValue)
                    return childResult;
            }

            return func.Invoke(current);
        }
    }
}