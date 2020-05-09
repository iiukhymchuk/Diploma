using System;
using System.Collections.Generic;

namespace SetTheory
{
    public static class Search
    {
        public static void DFSPostOrder(
            this Expression graph,
            Action<Expression> action,
            HashSet<Expression> visited = null)
        {
            visited ??= new HashSet<Expression>();
            if (visited.Contains(graph))
                return;

            visited.Add(graph);

            foreach (var child in graph.Children)
            {
                child.DFSPostOrder(action, visited);
            }

            action?.Invoke(graph);
        }

        public static Result<Substitution> DFSPostOrder(
            this Expression graph,
            Func<Expression, Result<Substitution>> func,
            HashSet<Expression> visited = null)
        {
            visited ??= new HashSet<Expression>();
            if (visited.Contains(graph))
                return Result<Substitution>.Empty();

            visited.Add(graph);

            foreach (var child in graph.Children)
            {
                var childResult = child.DFSPostOrder(func, visited);
                if (childResult.HasValue)
                    return childResult;
            }

            var result = func.Invoke(graph);
            if (result.HasValue)
            {
                result.Value.Initial = graph;
                return result;
            }

            return Result<Substitution>.Empty();
        }
    }
}