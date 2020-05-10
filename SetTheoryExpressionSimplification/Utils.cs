using System;
using System.Collections.Generic;
using System.Linq;

namespace SetTheory
{
    static class Utils
    {
        internal static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> list, int length)
        {
            if (!list.Any()) return Array.Empty<IEnumerable<T>>();

            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(
                    t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        internal static bool ExprEquals(Expression first, Expression second)
        {
            if (first.Type != second.Type) return false;
            if (first.Value != second.Value) return false;

            return CollectionsEquals(first.Children, second.Children, ExprEquals);
        }

        internal static bool CollectionsEquals<T>(T[] first, T[] second, Func<T, T, bool> equationCriteria)
        {
            if (first.Length != second.Length)
                return false;
            if (first.Length == 0)
                return true;

            return first
                .Zip(second, (x, y) => (x, y))
                .Select(pair => equationCriteria(pair.x, pair.y))
                .All(match => match);
        }
    }
}