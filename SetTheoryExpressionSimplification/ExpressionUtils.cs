using System;
using System.Collections.Generic;
using System.Linq;

namespace SetTheory
{
    public static class ExpressionUtils
    {
        public static void DFSPostOrder<T>(
            this Expression graph,
            Action<T> action,
            HashSet<Expression> visited = null)
        {
            visited ??= new HashSet<Expression>();
            if (visited.Contains(graph))
                return;

            visited.Add(graph);

            foreach (var child in graph.Children)
            {
                graph.DFSPostOrder<T>(action, visited);
            }

            action?.Invoke((T)(object)graph.Children);
        }

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
                graph.DFSPostOrder(action, visited);
            }

            action?.Invoke(graph);
        }

        // it is not Post order, rewrite
        public static (bool, Expression, Expression, Expression, string) DFSPostOrder2(
            this Expression graph,
            Func<Expression, (bool, Expression, Expression, string)> action)
        {
            var visited = new HashSet<Expression>();
            var stack = new Stack<Expression>();

            stack.Push(graph);

            while (stack.Count > 0)
            {
                var vertex = stack.Pop();

                if (visited.Contains(vertex))
                    continue;

                visited.Add(vertex);

                foreach (var child in graph.Children)
                    if (!visited.Contains(child))
                        stack.Push(child);

                var (success, substitution, resulting, description) = action.Invoke(vertex);
                if (success)
                    return (success, vertex, substitution, resulting, description);
            }

            return (false, null, null, null, null);
        }

        public static bool ExprEquals(Expression first, Expression second)
        {
            if (first.Type != second.Type) return false;
            if (first.Value != second.Value) return false;
            if (first.Children.Length != second.Children.Length) return false;

            foreach (var two in first.Children.Zip(second.Children, (x, y) => (x, y)))
            {
                if (!ExprEquals(two.x, two.y))
                    return false;
            }
            return true;
        }
    }
}