using DiscreteMath.Core.Language;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscreteMath.Core.Utils
{
    static class TreeUtils
    {
        internal static List<List<T>> GetPermutations<T>(this IEnumerable<T> list, int length)
        {
            if (!list.Any()) return new List<List<T>>();

            if (length == 1) return list.Select(t => new List<T> { t }).ToList();

            return list.GetPermutations(length - 1)
                .SelectMany(
                    t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new List<T> { t2 }).ToList())
                .ToList();
        }

        internal static IEnumerable<IEnumerable<Expression>> SplitInTwo(this IEnumerable<Expression> expressions, Expression prototype)
        {
            var count = expressions.Count();

            if (count <= 0) throw new ArgumentException();

            else if (count == 1 || count == 2)
                yield return expressions;

            else if (count > 2)
            {
                // number of separations in two parts is always one less than count in list
                for (int i = 1; i < count; i++)
                {
                    var part1 = ToSingleExpression(expressions.Take(i).ToList());
                    var part2 = ToSingleExpression(expressions.Skip(i).ToList());

                    yield return new List<Expression> { part1, part2 };
                }
            }

            Expression ToSingleExpression(List<Expression> list)
            {
                if (list.Count() == 1)
                    return list.First();

                var e = prototype.Copy();
                e.Children = list.ToArray();
                return e;
            }
        }

        internal static bool StrictEquals(this Expression first, Expression second)
        {
            if (first.Type != second.Type) return false;
            if (first.Value != second.Value) return false;

            return first.Children.PairsComply(second.Children, StrictEquals);
        }

        internal static bool PairsComply<T>(this T[] first, T[] second, Func<T, T, bool> complyCriteria)
        {
            if (first.Length != second.Length)
                return false;
            if (first.Length == 0)
                return true;

            return first
                .Zip(second, (x, y) => (x, y))
                .All(pair => complyCriteria(pair.x, pair.y));
        }

        internal static Expression SubstituteNode(this Expression expr, Guid id, Expression substitution)
        {
            return expr
                .AsTree()
                .ChangeTree(x => x.Id == id, _ => substitution)
                .AsExpression();
        }

        internal static Expression SubstituteNode(this Expression expr,
            Func<Expression, bool> predicate,
            Func<Expression, Expression> selector)
        {
            return expr
                .AsTree()
                .ChangeTree(predicate, selector)
                .AsExpression();
        }
    }
}